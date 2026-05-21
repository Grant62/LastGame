/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
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
    internal class UpdateCategoriesFromModelCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            IPackageManagerModel model = this.GetModel<IPackageManagerModel>();

            IPackageTypeConfigModel packageTypeConfigModel = this.GetModel<IPackageTypeConfigModel>();
            List<string> categories = model.Repositories.Select(p => p.type).Distinct()
                .Select(t => packageTypeConfigModel.GetFullTypeName(t))
                .ToList();
            categories.Insert(0, "All");
            PackageManagerState.Categories.Value = categories;
        }
    }
}
#endif