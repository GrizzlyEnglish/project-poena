﻿using MonoGame.Extended.Input.InputListeners;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Scene
{
    public interface ISceneLayer : INodeObject
    {
        void InjectScene(AbstractScene scene);
        void WindowResizeEvent();
    }
}