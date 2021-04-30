using Poena.Core.Common.Enums;
using Poena.Core.Entity.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Screen.Battle.Components
{
    public class AttackingComponent : IComponent
    {
        public AttackType AttackType { get; set; }

        public void Initialize()
        {
        }
    }
}
