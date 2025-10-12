using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Repository
{
     public interface IBillTypeRepo
    {
        public Task<List<BillType>> GetAllBillType();
    }
}
