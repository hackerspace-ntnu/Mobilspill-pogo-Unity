using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models {
    public interface IDictionaryObject {

        Dictionary<string, Object> ToDictionary();

    }
}