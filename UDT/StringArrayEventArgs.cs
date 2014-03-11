using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMBACore.UDT
{
    public class ParameterEventArgs : EventArgs
    {
        private IEnumerable<string> _UIDs;
        public IEnumerable<string> UIDs
        {
            get
            {
                return _UIDs;
            }
        }
        public ParameterEventArgs(IEnumerable<string> uids)
        {
            _UIDs = uids;
        }
    }
}
