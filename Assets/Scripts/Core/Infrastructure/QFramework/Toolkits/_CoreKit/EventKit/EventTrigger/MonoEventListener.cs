using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QFramework
{
    public enum MonoEventType
    {
        PointerClick,
        PointerDown,
        PointerUp,
        PointerEnter,
        PointerExit,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
        Scroll,
        Select,
        Deselect,
        Move,
        Submit,
        Cancel,
        InitializePotentialDrag,
        UpdateSelected,
        CollisionEnter,
        CollisionExit,
        CollisionStay,
        CollisionEnter2D,
        CollisionExit2D,
        CollisionStay2D,
        TriggerEnter,
        TriggerExit,
        TriggerStay,
        TriggerEnter2D,
        TriggerExit2D,
        TriggerStay2D,
        BecameVisible,
        BecameInvisible
    }

    public class MonoEventListener : MonoBehaviour,
        IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IDropHandler, IScrollHandler,
        ISelectHandler, IDeselectHandler, IMoveHandler,
        ISubmitHandler, ICancelHandler,
        IInitializePotentialDragHandler, IUpdateSelectedHandler
    {
        private Dictionary<int, Delegate> mEvents;

        private Dictionary<int, Delegate> Events
        {
            get
            {
                if (mEvents == null)
                {
                    mEvents = new Dictionary<int, Delegate>();
                }

                return mEvents;
            }
        }

        public IUnRegister Register(MonoEventType eventType, Action callback)
        {
            int key = (int)eventType;
            if (Events.TryGetValue(key, out Delegate existing))
            {
                Events[key] = Delegate.Combine(existing, callback);
            }
            else
            {
                Events[key] = callback;
            }

            return new CustomUnRegister(() => UnRegister(eventType, callback));
        }

        public IUnRegister Register<T>(MonoEventType eventType, Action<T> callback)
        {
            int key = (int)eventType;
            if (Events.TryGetValue(key, out Delegate existing))
            {
                Events[key] = Delegate.Combine(existing, callback);
            }
            else
            {
                Events[key] = callback;
            }

            return new CustomUnRegister(() => UnRegister(eventType, callback));
        }

        public void UnRegister(MonoEventType eventType, Action callback)
        {
            int key = (int)eventType;
            if (Events.TryGetValue(key, out Delegate existing))
            {
                Events[key] = Delegate.Remove(existing, callback);
            }
        }

        public void UnRegister<T>(MonoEventType eventType, Action<T> callback)
        {
            int key = (int)eventType;
            if (Events.TryGetValue(key, out Delegate existing))
            {
                Events[key] = Delegate.Remove(existing, callback);
            }
        }

        private void Trigger<T>(MonoEventType eventType, T arg)
        {
            int key = (int)eventType;
            if (mEvents != null && mEvents.TryGetValue(key, out Delegate del))
            {
                (del as Action<T>)?.Invoke(arg);
            }
        }

        private void Trigger(MonoEventType eventType)
        {
            int key = (int)eventType;
            if (mEvents != null && mEvents.TryGetValue(key, out Delegate del))
            {
                (del as Action)?.Invoke();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Trigger(MonoEventType.PointerClick, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Trigger(MonoEventType.PointerDown, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Trigger(MonoEventType.PointerUp, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Trigger(MonoEventType.PointerEnter, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Trigger(MonoEventType.PointerExit, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Trigger(MonoEventType.BeginDrag, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Trigger(MonoEventType.Drag, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Trigger(MonoEventType.EndDrag, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Trigger(MonoEventType.Drop, eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            Trigger(MonoEventType.Scroll, eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            Trigger(MonoEventType.Select, eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Trigger(MonoEventType.Deselect, eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            Trigger(MonoEventType.Move, eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Trigger(MonoEventType.Submit, eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            Trigger(MonoEventType.Cancel, eventData);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            Trigger(MonoEventType.InitializePotentialDrag, eventData);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            Trigger(MonoEventType.UpdateSelected, eventData);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Trigger(MonoEventType.CollisionEnter, collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            Trigger(MonoEventType.CollisionExit, collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            Trigger(MonoEventType.CollisionStay, collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Trigger(MonoEventType.CollisionEnter2D, collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            Trigger(MonoEventType.CollisionExit2D, collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            Trigger(MonoEventType.CollisionStay2D, collision);
        }

        private void OnTriggerEnter(Collider collider)
        {
            Trigger(MonoEventType.TriggerEnter, collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            Trigger(MonoEventType.TriggerExit, collider);
        }

        private void OnTriggerStay(Collider collider)
        {
            Trigger(MonoEventType.TriggerStay, collider);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Trigger(MonoEventType.TriggerEnter2D, collider);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            Trigger(MonoEventType.TriggerExit2D, collider);
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            Trigger(MonoEventType.TriggerStay2D, collider);
        }

        private void OnBecameVisible()
        {
            Trigger(MonoEventType.BecameVisible);
        }

        private void OnBecameInvisible()
        {
            Trigger(MonoEventType.BecameInvisible);
        }
    }

    public static class MonoEventListenerExtension
    {
        public static IUnRegister OnEvent(this Component self, MonoEventType eventType, Action callback)
        {
            return self.GetOrAddComponent<MonoEventListener>().Register(eventType, callback);
        }

        public static IUnRegister OnEvent<T>(this Component self, MonoEventType eventType, Action<T> callback)
        {
            return self.GetOrAddComponent<MonoEventListener>().Register(eventType, callback);
        }

        public static IUnRegister OnEvent(this GameObject self, MonoEventType eventType, Action callback)
        {
            return self.GetOrAddComponent<MonoEventListener>().Register(eventType, callback);
        }

        public static IUnRegister OnEvent<T>(this GameObject self, MonoEventType eventType, Action<T> callback)
        {
            return self.GetOrAddComponent<MonoEventListener>().Register(eventType, callback);
        }
    }
}