using System;

namespace BCS.CORE.VR.Network
{
    /// <summary>
    /// Структура для синхронизации активности частей тела (вкл/выкл)
    /// </summary>
    [Serializable]
    public struct BodyActive
    {
        public int role;
        public bool active;

        public BodyActive(int role, bool active)
        {
            this.role = role;
            this.active = active;
        }

        public override string ToString()
        {
            return "Role: " + role + " active: " + active;
        }
    }
}