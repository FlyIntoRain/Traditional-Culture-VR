using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentHuntGameManager: MonoBehaviour
{
	[SerializeField]
	private RectTransform existingGridRoot; // 场景中已摆好子物体）

	[SerializeField]
	private bool bindExistingGridOnStart = false; // 启动时自动绑定已有Grid

	[SerializeField]
	private Vector2 gridCellSize = new Vector2(160, 160);

	[SerializeField]
	private Vector2 gridSpacing = new Vector2(12, 12);

	[SerializeField]
	private Color colorNear = Color.yellow;

	[SerializeField]
	private Color colorFar = Color.red;

	[SerializeField]
	private Color colorCorrect = Color.green;

	[SerializeField]
	private Color colorDefault = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField]
    private RectTransform gameLayer;
	private GridLayoutGroup gridLayout;
	private readonly List<FragmentCell> cells = new List<FragmentCell>();
	private Vector2Int fragmentPos;
	private bool gameOver;

	[ContextMenu("ShowGame")]
	public void ShowGame()
	{
		RestartGame();
		if (gameLayer != null)
		{
			gameLayer.gameObject.SetActive(true);
			gameLayer.parent.gameObject.SetActive(true);
		}
	}

	public void HideGame()
	{
		if (gameLayer != null)
		{
			gameLayer.gameObject.SetActive(false);
            gameLayer.parent.gameObject.SetActive(false);
        }
	}

	public void RestartGame()
	{
		gameOver = false;
		fragmentPos = new Vector2Int(Random.Range(0, 3), Random.Range(0, 3));
		for (int i = 0; i < cells.Count; i++)
		{
			cells[i].ResetVisual(colorDefault, true);
		}
	}

	private void Start()
	{
		if (bindExistingGridOnStart && existingGridRoot != null)
		{
			SetupFromExistingGrid(existingGridRoot, 3);
		}

	}

	[ContextMenu("SetupFromExistingGrid (use existingGridRoot)")]
	public void SetupFromExistingGrid()
	{
		if (existingGridRoot == null)
		{
			Debug.LogError("existingGridRoot 未指定，无法绑定已有Grid。");
			return;
		}
		SetupFromExistingGrid(existingGridRoot, 3);
	}

	public void SetupFromExistingGrid(RectTransform gridRoot, int columns)
	{
		if (gridRoot == null)
		{
			Debug.LogError("gridRoot 为空，无法绑定。");
			return;
		}

		// 将游戏图层指向已有Grid的根（用于Show/Hide）
		gameLayer = gridRoot;
		gridLayout = gridRoot.GetComponent<GridLayoutGroup>();

		cells.Clear();
		int childCount = gridRoot.childCount;
		if (columns <= 0) columns = 3;
		int rows = Mathf.CeilToInt(childCount / (float)columns);

		for (int i = 0; i < childCount; i++)
		{
			var child = gridRoot.GetChild(i) as RectTransform;
			if (child == null) continue;

			int x = i % columns;
			int y = i / columns;
			var image = child.GetComponent<Image>();
			if (image == null) image = child.gameObject.AddComponent<Image>();
			image.color = colorDefault;

			var button = child.GetComponent<Button>();
			if (button == null) button = child.gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.ColorTint;
			var colors = button.colors;
			colors.highlightedColor = new Color(1f, 1f, 1f, 0.9f);
			colors.pressedColor = new Color(1f, 1f, 1f, 0.8f);
			button.colors = colors;

			var cell = child.GetComponent<FragmentCell>();
			if (cell == null) cell = child.gameObject.AddComponent<FragmentCell>();
			cell.Init(this, new Vector2Int(x, y), image, button);
			cells.Add(cell);
		}

		// 初始化一局
		RestartGame();
	}


	internal void OnCellClicked(FragmentCell cell)
	{
		if (gameOver) return;

		var pos = cell.GridPosition;
		if (pos == fragmentPos)
		{
			cell.SetColor(colorCorrect);
			gameOver = true;
			DisableAllCellsExcept(null);
			HideGame();
			Debug.Log("找到碎片，游戏结束！");
			return;
		}

		bool isAdjacent = IsOrthogonallyAdjacent(pos, fragmentPos);
		cell.SetColor(isAdjacent ? colorNear : colorFar);
	}

	private static bool IsOrthogonallyAdjacent(Vector2Int a, Vector2Int b)
	{
		int dx = Mathf.Abs(a.x - b.x);
		int dy = Mathf.Abs(a.y - b.y);
		return (dx + dy) == 1; // 仅上下左右
	}

	private void DisableAllCellsExcept(FragmentCell except)
	{
		for (int i = 0; i < cells.Count; i++)
		{
			if (cells[i] == null) continue;
			if (except != null && cells[i] == except) continue;
			cells[i].SetInteractable(false);
		}
	}
}


