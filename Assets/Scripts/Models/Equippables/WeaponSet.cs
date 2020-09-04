using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iam.Scripts.Models.Equippables
{
    public class WeaponSet
    {
        public string Name { get; set; }
        public WeaponTemplate MainWeapon { get; set; }
        public WeaponTemplate SecondaryWeapon { get; set; }
        public List<Weapon> GetWeapons()
        {
            List<Weapon> list = new List<Weapon>();
            list.Add(new Weapon(MainWeapon));
            if(SecondaryWeapon != null)
            {
                list.Add(new Weapon(SecondaryWeapon));
            }
            return list;
        }
    }
}
