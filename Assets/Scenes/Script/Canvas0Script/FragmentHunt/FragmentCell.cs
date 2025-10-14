using UnityEngine;
using UnityEngine.UI;

public class FragmentCell : MonoBehaviour
{
	private FragmentHuntGameManager manager;
	private Image image;
	private Button button;

	public Vector2Int GridPosition { get; private set; }

	public void Init(FragmentHuntGameManager mgr, Vector2Int gridPos, Image img, Button btn)
	{
		manager = mgr;
		GridPosition = gridPos;
		image = img;
		button = btn;
		button.onClick.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		if (button != null)
		{
			button.onClick.RemoveListener(OnClick);
		}
	}

	private void OnClick()
	{
		if (manager != null)
		{
			manager.OnCellClicked(this);
		}
	}

	public void SetColor(Color color)
	{
		if (image != null)
		{
			image.color = color;
		}
	}

	public void SetInteractable(bool interactable)
	{
		if (button != null)
		{
			button.interactable = interactable;
		}
	}

	public void ResetVisual(Color defaultColor, bool interactable)
	{
		SetColor(defaultColor);
		SetInteractable(interactable);
	}
}


