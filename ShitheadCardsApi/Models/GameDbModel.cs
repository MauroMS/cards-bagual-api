using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShitheadCardsApi.Models
{
    public class GameDbModel
    {
        [Key]
        public string Name { get; set; }

        public string GameJson { get; set; }
    }
}
