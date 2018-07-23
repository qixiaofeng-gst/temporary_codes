using System.Collections;

namespace PDPU
{
    public class TelemetryHash
    {
        protected readonly Hashtable hashTable;

        public Hashtable HashTable
        {
            get { return hashTable; }
        }

        public TelemetryHash()
        {
            hashTable = new Hashtable();
        }
    }
}
