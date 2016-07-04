using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
	public class UIItemDestroyBox : MonoBehaviour {
	
		private static UIItemDestroyBox m_Instance;
		
		[SerializeField] private UIWindow m_Window;
		[SerializeField] private UIItemSlot m_Slot;
		[SerializeField] private Text m_NameText;
		[SerializeField] private Text m_DescriptionText;
		
		private UIItemSlot m_SelectedSlot;
		
		protected void Awake()
		{
			if (m_Instance == null)
				m_Instance = this;
		}
		
		public static void Show(UIItemSlot slot)
		{
			if (m_Instance == null)
				return;
			
			// Assign the item slot
			m_Instance.AssignSlot(slot);
			
			// Show the window
			m_Instance.Show();
		}
		
		public void Show()
		{
			if (this.m_Window != null)
				this.m_Window.Show();
		}
		
		public void Hide()
		{
			if (this.m_Window != null)
				this.m_Window.Hide();
		}
		
		public void OnWindowTransitionComplete(UIWindow window, UIWindow.VisualState state)
		{
			if (state == UIWindow.VisualState.Hidden)
			{
				if (this.m_Slot != null)
					this.m_Slot.Unassign();
				
				if (this.m_NameText != null)
					this.m_NameText.text = "";
				
				if (this.m_DescriptionText != null)
					this.m_DescriptionText.text = "";
				
				this.m_SelectedSlot = null;
			}
		}
		
		/// <summary>
		/// Assigns the slot to the box.
		/// </summary>
		/// <param name="slot">Slot.</param>
		private void AssignSlot(UIItemSlot slot)
		{
			if (!slot.IsAssigned() || slot.GetItemInfo() == null)
				return;
				
			if (this.m_Slot != null)
				this.m_Slot.Assign(slot.GetItemInfo());
			
			if (this.m_NameText != null)
				this.m_NameText.text = slot.GetItemInfo().Name;
			
			if (this.m_DescriptionText != null)
				this.m_DescriptionText.text = slot.GetItemInfo().Subtype + " " + slot.GetItemInfo().Type;
			
			// Save a ref to the slot
			this.m_SelectedSlot = slot;
		}
		
		/// <summary>
		/// Confirm the item destruction.
		/// </summary>
		public void Confirm()
		{
			if (this.m_SelectedSlot != null)
			{
				this.m_SelectedSlot.Unassign();
			}
			
			// Hide the window
			this.Hide();
		}
	}
}