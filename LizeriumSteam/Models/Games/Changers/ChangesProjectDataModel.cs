/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 апреля 2026 08:31:42
 * Version: 1.0.32
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace LizeriumSteam.Models.Games.Changers
{
    [Serializable]
    public class ChangesProjectDataModel
    {
        public string Comment { get; set; }
        public List<CategoryChangeModel> Categories { get; set; }
        public List<ChangeModel> Updates { get; set; }
    }

    [Serializable]
    public class ChangeModel
    {
        public string name { get; set; }
        public List<ChangeDataModel> data { get; set; }
    }

    [Serializable]
    public class ChangeDataModel
    {
        public string category { get; set; }
        public CategoryChangeModel Category { get; set; }

        public List<string> values { get; set; }

        public CategoryChangeModel GetCategory(List<CategoryChangeModel> Categories)
        {
            return Categories.FirstOrDefault(it => it.name == category);
        }
    }

    [Serializable]
    public class CategoryChangeModel
    {
        public string name { get; set; }
        public string title { get; set; }
    }
}
