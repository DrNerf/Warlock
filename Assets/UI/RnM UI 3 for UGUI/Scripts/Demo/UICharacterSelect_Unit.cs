using UnityEngine;
using UnityEngine.UI.Tweens;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public class UICharacterSelect_Unit : Toggle {
		
		[SerializeField] private Text m_NameTextComponent;
		[SerializeField] private Text m_RaceTextComponent;
		[SerializeField] private Text m_ClassTextComponent;
		[SerializeField] private Text m_LevelTextComponent;
		[SerializeField] private Text m_LevelLabelTextComponent;
		
		[SerializeField] private ColorBlockExtended m_NameColors = ColorBlockExtended.defaultColorBlock;
		[SerializeField] private ColorBlockExtended m_RaceColors = ColorBlockExtended.defaultColorBlock;
		[SerializeField] private ColorBlockExtended m_ClassColors = ColorBlockExtended.defaultColorBlock;
		[SerializeField] private ColorBlockExtended m_LevelColors = ColorBlockExtended.defaultColorBlock;
		
		[SerializeField] private Button m_DeleteButton;
		[SerializeField] private bool m_DeleteButtonAlwaysVisible = false;
		[SerializeField] private float m_DeleteButtonFadeDuration = 0.1f;
		
		[SerializeField] private GameObject m_Separator;
		
		// Tween controls
		[System.NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UICharacterSelect_Unit()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected override void Start()
		{
			base.Start();
			this.DoStateTransition(SelectionState.Normal, true);
			this.onValueChanged.AddListener(OnActiveStateChange);
		}
		
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			this.ResetColors();
			
			// Prepare the delete button behaviour
			CanvasGroup cg = this.GetDeleteButtonCavnasGroup();
			
			if (cg != null)
			{
				if (this.m_DeleteButtonAlwaysVisible)
					cg.alpha = 1f;
				else
					cg.alpha = 0f;
			}
		}
#endif
		
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			
			// Fix layout groups
			if (Application.isPlaying)
			{
				foreach (LayoutGroup lg in this.gameObject.GetComponentsInChildren<LayoutGroup>())
				{
					lg.SetLayoutHorizontal();
					lg.SetLayoutVertical();
				}
			}
		}
		
		protected override void InstantClearState()
		{
			base.InstantClearState();
			this.ResetColors();
			
			// Reset the alpha of the delete button to zero
			if (this.GetDeleteButtonCavnasGroup() != null)
				this.GetDeleteButtonCavnasGroup().alpha = (this.m_DeleteButtonAlwaysVisible ? 1f : 0f);
		}
		
		private void ResetColors()
		{
			if (this.m_NameTextComponent != null) this.m_NameTextComponent.canvasRenderer.SetColor((this.isOn ? this.m_NameColors.activeColor : this.m_NameColors.normalColor) * this.m_NameColors.colorMultiplier);
			if (this.m_LevelLabelTextComponent != null) this.m_LevelLabelTextComponent.canvasRenderer.SetColor((this.isOn ? this.m_LevelColors.activeColor : this.m_LevelColors.normalColor) * this.m_LevelColors.colorMultiplier);
			if (this.m_LevelTextComponent != null) this.m_LevelTextComponent.canvasRenderer.SetColor((this.isOn ? this.m_LevelColors.activeColor : this.m_LevelColors.normalColor) * this.m_LevelColors.colorMultiplier);
			if (this.m_RaceTextComponent != null) this.m_RaceTextComponent.canvasRenderer.SetColor((this.isOn ? this.m_RaceColors.activeColor : this.m_RaceColors.normalColor) * this.m_RaceColors.colorMultiplier);
			if (this.m_ClassTextComponent != null) this.m_ClassTextComponent.canvasRenderer.SetColor((this.isOn ? this.m_ClassColors.activeColor : this.m_ClassColors.normalColor) * this.m_ClassColors.colorMultiplier);
		}
	
		protected virtual void OnActiveStateChange(bool isOn)
		{
			this.DoStateTransition(this.currentSelectionState, false);
		}
		
		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			
			// Check if the script is enabled
			if (!this.enabled || !this.gameObject.activeInHierarchy || !Application.isPlaying)
				return;
			
			Color nameColor = this.m_NameColors.normalColor;
			Color raceColor = this.m_RaceColors.normalColor;
			Color classColor = this.m_ClassColors.normalColor;
			Color levelColor = this.m_LevelColors.normalColor;
			
			// Prepare the transition values
			if (state == SelectionState.Disabled)
			{
				nameColor = this.m_NameColors.disabledColor;
				raceColor = this.m_RaceColors.disabledColor;
				classColor = this.m_ClassColors.disabledColor;
				levelColor = this.m_LevelColors.disabledColor;
			}
			else if (this.isOn)
			{
				switch (state)
				{
				case SelectionState.Normal:
					nameColor = this.m_NameColors.activeColor;
					raceColor = this.m_RaceColors.activeColor;
					classColor = this.m_ClassColors.activeColor;
					levelColor = this.m_LevelColors.activeColor;
					break;
				case SelectionState.Highlighted:
					nameColor = this.m_NameColors.activeHighlightedColor;
					raceColor = this.m_RaceColors.activeHighlightedColor;
					classColor = this.m_ClassColors.activeHighlightedColor;
					levelColor = this.m_LevelColors.activeHighlightedColor;
					break;
				case SelectionState.Pressed:
					nameColor = this.m_NameColors.activePressedColor;
					raceColor = this.m_RaceColors.activePressedColor;
					classColor = this.m_ClassColors.activePressedColor;
					levelColor = this.m_LevelColors.activePressedColor;
					break;
				}
			}
			else
			{
				switch (state)
				{
				case SelectionState.Normal:
					nameColor = this.m_NameColors.normalColor;
					raceColor = this.m_RaceColors.normalColor;
					classColor = this.m_ClassColors.normalColor;
					levelColor = this.m_LevelColors.normalColor;
					break;
				case SelectionState.Highlighted:
					nameColor = this.m_NameColors.highlightedColor;
					raceColor = this.m_RaceColors.highlightedColor;
					classColor = this.m_ClassColors.highlightedColor;
					levelColor = this.m_LevelColors.highlightedColor;
					break;
				case SelectionState.Pressed:
					nameColor = this.m_NameColors.pressedColor;
					raceColor = this.m_RaceColors.pressedColor;
					classColor = this.m_ClassColors.pressedColor;
					levelColor = this.m_LevelColors.pressedColor;
					break;
				}
			}
			
			// Do the transition
			if (this.m_NameTextComponent != null)
				this.StartColorTween(this.m_NameTextComponent, nameColor * this.m_NameColors.colorMultiplier, (instant ? 0f : this.m_NameColors.fadeDuration));
			
			if (this.m_RaceTextComponent != null)
				this.StartColorTween(this.m_RaceTextComponent, raceColor * this.m_RaceColors.colorMultiplier, (instant ? 0f : this.m_RaceColors.fadeDuration));
			
			if (this.m_ClassTextComponent != null)
				this.StartColorTween(this.m_ClassTextComponent, classColor * this.m_ClassColors.colorMultiplier, (instant ? 0f : this.m_ClassColors.fadeDuration));
			
			if (this.m_LevelTextComponent != null)
				this.StartColorTween(this.m_LevelTextComponent, levelColor * this.m_LevelColors.colorMultiplier, (instant ? 0f : this.m_LevelColors.fadeDuration));
			
			if (this.m_LevelLabelTextComponent != null)
				this.StartColorTween(this.m_LevelLabelTextComponent, levelColor * this.m_LevelColors.colorMultiplier, (instant ? 0f : this.m_LevelColors.fadeDuration));
				
			// Handle the delete button visibility
			if (!this.m_DeleteButtonAlwaysVisible)
			{
				CanvasGroup cg = this.GetDeleteButtonCavnasGroup();
				
				// Check if we have a canvas group
				if (cg != null)
				{
					bool showDelete = (state == SelectionState.Normal || state == SelectionState.Disabled) ? false : true;
					
					if (instant || this.m_DeleteButtonFadeDuration == 0f)
					{
						cg.alpha = (showDelete ? 1f : 0f);
					}
					else
					{
						this.TweenDeleteButtonAlpha((showDelete ? 1f : 0f), this.m_DeleteButtonFadeDuration, true);
					}
					
					// Disable the canvas group interaction
					cg.blocksRaycasts = (showDelete ? true : false);
					cg.interactable = (showDelete ? true : false);
				}
			}
		}
		
		/// <summary>
		/// Starts the color tween.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartColorTween(Text target, Color targetColor, float duration)
		{
			if (target == null)
				return;
			
			if (duration == 0f)
			{
				target.canvasRenderer.SetColor(targetColor);
			}
			else
			{
				target.CrossFadeColor(targetColor, duration, true, true);
			}
		}
		
		/// <summary>
		/// Gets the delete button cavnas group.
		/// </summary>
		/// <returns>The delete button cavnas group.</returns>
		protected CanvasGroup GetDeleteButtonCavnasGroup()
		{
			if (this.m_DeleteButton != null)
			{
				CanvasGroup cg = this.m_DeleteButton.gameObject.GetComponent<CanvasGroup>();
				return (cg == null) ? this.m_DeleteButton.gameObject.AddComponent<CanvasGroup>() : cg;
			}
			
			return null;
		}
		
		/// <summary>
		/// Sets the delete button alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		protected void SetDeleteButtonAlpha(float alpha)
		{
			if (this.GetDeleteButtonCavnasGroup() != null)
				this.GetDeleteButtonCavnasGroup().alpha = alpha;
		}
		
		/// <summary>
		/// Tweens the delete button alpha.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		public void TweenDeleteButtonAlpha(float targetAlpha, float duration, bool ignoreTimeScale)
		{
			if (this.GetDeleteButtonCavnasGroup() == null)
				return;
			
			float currentAlpha = this.GetDeleteButtonCavnasGroup().alpha;
			
			if (currentAlpha.Equals(targetAlpha))
				return;
			
			var floatTween = new FloatTween { duration = duration, startFloat = currentAlpha, targetFloat = targetAlpha };
			floatTween.AddOnChangedCallback(SetDeleteButtonAlpha);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			this.m_FloatTweenRunner.StartTween(floatTween);
		}
		
		/// <summary>
		/// Sets the name of the unit.
		/// </summary>
		/// <param name="name">Name.</param>
		public void SetName(string name)
		{
			if (this.m_NameTextComponent != null)
				this.m_NameTextComponent.text = name;
		}
		
		/// <summary>
		/// Sets the level of the unit.
		/// </summary>
		/// <param name="level">Level.</param>
		public void SetLevel(int level)
		{
			if (this.m_LevelTextComponent != null)
				this.m_LevelTextComponent.text = level.ToString();
		}
		
		/// <summary>
		/// Sets the class of the unit.
		/// </summary>
		public void SetClass(string mClass)
		{
			if (this.m_ClassTextComponent != null)
				this.m_ClassTextComponent.text = mClass;
		}
		
		/// <summary>
		/// Sets the race of the unit.
		/// </summary>
		public void SetRace(string mRace)
		{
			if (this.m_RaceTextComponent != null)
				this.m_RaceTextComponent.text = mRace;
		}
		
		/// <summary>
		/// Toggles the separator's game object active state.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		public void ToggleSeparator(bool state)
		{
			if (this.m_Separator != null)
				this.m_Separator.SetActive(state);
		}
	}
}