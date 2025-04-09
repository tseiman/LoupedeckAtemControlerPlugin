using System;

using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;

namespace Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel
{


    public class MultiWheelDispatch
    {

        private class DispatcherData
        {
            public Type dispatcherType { get; }
            public IMultiWheelDispatchable multiWheelDispatchable { get; }
            public IMultiWheelAtemAdjustment multiWheelAtemAdjustment { get; }

            public DispatcherData(Type t, IMultiWheelDispatchable imwd, IMultiWheelAtemAdjustment imwaa)
            {
                this.dispatcherType = t;
                this.multiWheelDispatchable = imwd;
                this.multiWheelAtemAdjustment = imwaa;
            }

        }

        private readonly AtemControlInterface _atemControlInterface;


        private readonly Dictionary<Type, DispatcherData> _dispatchables = new();

        private DispatcherData _activeDispatcher;


        public MultiWheelDispatch()
        {
            this._atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
        }



        public void ApplyAdjustment(Int32 diff) {
            if (this._activeDispatcher == null)
            {
                return;
            }

            // PluginLog.Verbose($"[MultiWheelDispatch] ApplyAdjustment: {diff}");
            this._activeDispatcher.multiWheelAtemAdjustment.ApplyAdjustment(diff);

        }

        public void InformActive(IMultiWheelDispatchable dispatchable)
        {
            this._activeDispatcher = this._dispatchables[dispatchable.GetType()];

            foreach (var (type, disp_obj) in this._dispatchables)
            {
                if (type != dispatchable.GetType())
                {
                    disp_obj.multiWheelDispatchable.Disengage();
                }
            }
        }

        public void InformInActive(IMultiWheelDispatchable dispatchable)
        {
            if (dispatchable.GetType() == this._activeDispatcher.dispatcherType)
            {
                this._activeDispatcher = null;
            }
        }


        public void RegisterDispatchable(IMultiWheelDispatchable dispatchable, IMultiWheelAtemAdjustment imwaa)
            => this._dispatchables[dispatchable.GetType()] = new DispatcherData(dispatchable.GetType(),dispatchable, imwaa);

    }
}

