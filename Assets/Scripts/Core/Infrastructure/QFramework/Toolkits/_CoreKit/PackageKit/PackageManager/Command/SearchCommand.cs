/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    internal class SearchCommand : AbstractCommand
    {
        private readonly string mKey;

        public SearchCommand(string key)
        {
            mKey = key.ToLower();
        }

        protected override void OnExecute()
        {
            IPackageManagerModel model = this.GetModel<IPackageManagerModel>();
            int categoryIndex = PackageManagerState.CategoryIndex.Value;
            List<string> categories = PackageManagerState.Categories.Value;
            int accessRightIndex = PackageManagerState.AccessRightIndex.Value;

            List<PackageRepository> repositories = model
                .Repositories
                .Where(p => p.name.ToLower().Contains(mKey))
                .Where(p => categoryIndex == 0 || p.type.ToString() == categories[categoryIndex])
                .Where(p => accessRightIndex == 0 ||
                            accessRightIndex == 1 && p.accessRight == "public" ||
                            accessRightIndex == 2 && p.accessRight == "private"
                )
                .OrderBy(p => p.name)
                .ToList();

            PackageManagerState.PackageRepositories.Value = repositories;
        }
    }
}
#endif