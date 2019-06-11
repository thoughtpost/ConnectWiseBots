using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace Thoughtpost.Bots.Shared
{
    public class StateStorage
    {
        public StateStorage(IStorage storage)
        {
            this.State = new UserState(storage);
            this.UserConfigAccessor = State.CreateProperty<UserConfig>("UserConfigState");
        }

        public IStatePropertyAccessor<UserConfig> UserConfigAccessor { get; set; }
        public UserState State { get; set; }
    }

    public class UserConfig
    {
        public string Name { get; set; }
        public bool Prompted { get; set; }
        public string Id { get; set; }
        public string CompanyId { get; set; }
    }

}
