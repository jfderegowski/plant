using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fefek5.Systems.SelectableWithEventsSystem.Runtime
{
    public class ButtonWithEvents : Button, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<PublicSelectableState> onStateChange;
        public event Action<PointerEventData> onPointerEnter;
        public event Action<PointerEventData> onPointerExit;
        public event Action<PointerEventData> onPointerDown;
        public event Action<PointerEventData> onPointerUp;
        public event Action<AxisEventData> onMove;
        public event Action<PointerEventData> onBeginDrag;
        public event Action<PointerEventData> onDrag;
        public event Action<PointerEventData> onEndDrag;

        public PublicSelectableState CurrentState => currentSelectionState switch {
            SelectionState.Normal => PublicSelectableState.Normal,
            SelectionState.Highlighted => PublicSelectableState.Highlighted,
            SelectionState.Pressed => PublicSelectableState.Pressed,
            SelectionState.Selected => PublicSelectableState.Selected,
            SelectionState.Disabled => PublicSelectableState.Disabled,
            _ => throw new ArgumentOutOfRangeException()
        };

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            onStateChange?.Invoke(SelectableStateToPublicSelectableState(state));
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            onPointerEnter?.Invoke(eventData);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            onPointerExit?.Invoke(eventData);
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onPointerDown?.Invoke(eventData);
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onPointerUp?.Invoke(eventData);
        }
        
        public override void OnMove(AxisEventData axisEventData)
        {
            base.OnMove(axisEventData);
            onMove?.Invoke(axisEventData);
        }

        public virtual void OnBeginDrag(PointerEventData eventData) => onBeginDrag?.Invoke(eventData);

        public virtual void OnDrag(PointerEventData eventData) => onDrag?.Invoke(eventData);

        public virtual void OnEndDrag(PointerEventData eventData) => onEndDrag?.Invoke(eventData);
        
        private static PublicSelectableState SelectableStateToPublicSelectableState(SelectionState state) =>
            state switch
            {
                SelectionState.Normal => PublicSelectableState.Normal,
                SelectionState.Highlighted => PublicSelectableState.Highlighted,
                SelectionState.Pressed => PublicSelectableState.Pressed,
                SelectionState.Selected => PublicSelectableState.Selected,
                SelectionState.Disabled => PublicSelectableState.Disabled,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
    }
}