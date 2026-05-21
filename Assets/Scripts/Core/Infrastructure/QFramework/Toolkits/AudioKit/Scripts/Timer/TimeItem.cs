/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 ****************************************************************************/

using System;

namespace QFramework
{
    public class TimeItem : IBinaryHeapElement, IPoolable, IPoolType
    {
        /*
         * tick:当前第几次
         */

        private float mDelayTime;
        private int mRepeatCount;
        private int mCallbackTick;

        public static TimeItem Allocate(Action<int> callback, float delayTime, int repeatCount = 1)
        {
            TimeItem item = SafeObjectPool<TimeItem>.Instance.Allocate();
            item.Set(callback, delayTime, repeatCount);
            return item;
        }

        public void Set(Action<int> callback, float delayTime, int repeatCount)
        {
            mCallbackTick = 0;
            this.callback = callback;
            mDelayTime = delayTime;
            mRepeatCount = repeatCount;
        }

        public void OnTimeTick()
        {
            if (callback != null)
            {
                callback(++mCallbackTick);
            }

            if (mRepeatCount > 0)
            {
                --mRepeatCount;
            }
        }

        public Action<int> callback { get; private set; }

        public float SortScore { get; set; }

        public int HeapIndex { get; set; }

        public bool isEnable { get; private set; } = true;

        public bool IsRecycled { get; set; }

        public void Cancel()
        {
            if (isEnable)
            {
                isEnable = false;
                callback = null;
            }
        }

        public bool NeedRepeat()
        {
            if (mRepeatCount == 0)
            {
                return false;
            }

            return true;
        }

        public float DelayTime()
        {
            return mDelayTime;
        }

        public void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement
        {
            heap.RebuildAtIndex(HeapIndex);
        }

        public void OnRecycled()
        {
            mCallbackTick = 0;
            callback = null;
            isEnable = true;
            HeapIndex = 0;
        }

        public void Recycle2Cache()
        {
            //超出缓存最大值
            SafeObjectPool<TimeItem>.Instance.Recycle(this);
        }
    }
}