using System.Collections.Generic;

namespace Main.GM
{
    public class GmHistory
    {
        private readonly List<string> mHistory = new();
        private int mIndex = -1;

        public void Add(string command)
        {
            if (mHistory.Count == 0 || mHistory[^1] != command)
                mHistory.Add(command);

            mIndex = -1;
        }

        public bool TryNavigateUp(out string command)
        {
            command = null;
            if (mHistory.Count == 0)
                return false;

            if (mIndex < mHistory.Count - 1)
            {
                mIndex++;
                command = mHistory[^(mIndex + 1)];
                return true;
            }

            return false;
        }

        public bool TryNavigateDown(out string command)
        {
            command = null;

            if (mIndex > 0)
            {
                mIndex--;
                command = mHistory[^(mIndex + 1)];
                return true;
            }

            if (mIndex == 0)
            {
                mIndex = -1;
                command = string.Empty;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            mHistory.Clear();
            mIndex = -1;
        }
    }
}