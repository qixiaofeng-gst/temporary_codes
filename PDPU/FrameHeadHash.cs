using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com
{
    class FrameHeadHash
    {
        protected readonly Hashtable hashTable;

        public Hashtable HashTable
        {
            get { return hashTable; }
        }

        public FrameHeadHash()
        {
            hashTable = new Hashtable();
        }
    }
}
