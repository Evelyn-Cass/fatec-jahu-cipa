using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.ViewModel
{
    public class DocumentWithUserMandateMaterialViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? DocumentCreationDate { get; set; }
        public string? Attachment { get; set; }
        public string? Status { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Mandate { get; set; }
        public string? Material { get; set; }
    }
}

//db.Documents.aggregate
//([
//    {
//        $lookup:
//    {
//    from: "users",
//            localField: "userId",
//            foreignField: "_id",
//            as: "user"
//        }
//},
//    {
//        $lookup:
//    {
//    from: "mandates",
//            localField: "mandateId",
//            foreignField: "_id",
//            as: "mandate"
//        }
//},
//    {
//        $lookup:
//    {
//    from: "materials",
//            localField: "materialId",
//            foreignField: "_id",
//            as: "material"
//        }
//},
//    {
//        $unwind: "$user"
//    },
//    {
//        $unwind: "$mandate"
//    },
//    {
//        $unwind: "$material"
//    },
//    {
//        $project:
//    {
//    _id: 1,
//            name: 1,
//            documentCreationDate: 1,
//            attachment: 1,
//            status: 1,
//            userName: "$user.name",
//            mandate: "$mandate.startYear",
//            material: "$material.description"
//        }
//}
//])