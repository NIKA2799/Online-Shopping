using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Command
{
    public interface ICommandModel<in TCommandModel>
      where TCommandModel : IEntityModel
    {
        int Insert(TCommandModel entityModel);
        void Update(int id, TCommandModel entityModel);
        void Delete(int id);
    }

}

