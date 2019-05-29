using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Vittigheder1
{
    public class Agent : BindableBase
    {
        string id;
        string codeName;
        string speciality;
        string assignment;

        public Agent()
        {
        }

        public Agent(string aId, string aName, string aSpeciality, string aAssignment)
        {
            id = aId;
            codeName = aName;
            speciality = aSpeciality;
            assignment = aAssignment;
        }


        /// <summary>
        /// Performs only a shallow copy
        /// </summary>
        /// <returns></returns>
        public Agent Clone()
        {
            return this.MemberwiseClone() as Agent;
        }

        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                SetProperty(ref id, value);
            }
        }

        public string CodeName
        {
            get
            {
                return codeName;
            }
            set
            {
                SetProperty(ref codeName, value);
            }
        }

        public string Speciality
        {
            get
            {
                return speciality;
            }
            set
            {
                SetProperty(ref speciality, value);
            }
        }

        public string Assignment
        {
            get
            {
                return assignment;
            }
            set
            {
                SetProperty(ref assignment, value);
            }
        }
    }
}
