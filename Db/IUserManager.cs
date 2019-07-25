using System;
using System.Collections.Generic;
using System.Text;

namespace Db
{
    public interface IUserManager
    {
        Guid GetUserGuid(string ctsEmail);
    }
}
