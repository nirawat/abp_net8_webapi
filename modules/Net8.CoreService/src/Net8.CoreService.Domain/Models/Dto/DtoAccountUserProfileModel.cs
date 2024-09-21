using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net8.CoreService.Models.Dto
{
    public class DtoAccountUserProfileModel
    {
        public int Id { get; set; }
        public DtoAccountUserProfilesModel Profile { get; set; }
    }
    public class DtoAccountUserProfilesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

}
