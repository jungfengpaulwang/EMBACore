using System;

namespace EMBACore.Event
{
    class DeliverActiveRecord
    {
        public DeliverActiveRecord()
        {

        }

        internal static void RaiseSendingEvent(object sender, DeliverActiveRecordEventArgs e)
        {
            if (DeliverActiveRecord.Received != null)
                DeliverActiveRecord.Received(sender, e);
        }

        internal static event EventHandler<DeliverActiveRecordEventArgs> Received;
    }
}
