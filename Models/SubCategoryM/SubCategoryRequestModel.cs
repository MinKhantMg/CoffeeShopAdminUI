﻿using System.ComponentModel.DataAnnotations;

namespace CoffeeShopAdmin.Models.SubCategoryM
{
    public class SubCategoryRequestModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "SubCategory name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Select Category is required.")]
        public string CategoryId { get; set; }


        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool Status { get; set; }
    }
}
