using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class CinemaNearbyDTO : CinemaDTO
    {
        public double DistanceInMeters { get; set; }
    }
}
