using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class DeleteRequest
    {
        [Key]
        public Guid DeleteRequestId { get; set; }
        public Guid MovieId { get; set; }
        public string RequestedByAdminId { get; set; }
        public string Status { get; set; }
        public int ApprovedCount { get; set; }
        public DateTime? CreatedAt { get; set; }

        public ICollection<DeleteApproval> Approval { get; set; } = new List<DeleteApproval>();
    }



    public class DeleteApproval
    {
        [Key]
        public Guid ApprovalId { get; set; }
        public Guid DeleteRequestId { get; set; }
        public string ApprovedByAdminId { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DeleteRequest DeleteRequest { get; set; }

    }


}
