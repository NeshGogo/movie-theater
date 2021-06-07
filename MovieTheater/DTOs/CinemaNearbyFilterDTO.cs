using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class CinemaNearbyFilterDTO
    {
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }
        private double distanceInKms = 10;
        private readonly double maxDistanceInKms = 50;
        public double DistanceInKms 
        {
            get 
            {
                return distanceInKms;
            } 
            set
            {
                distanceInKms = (value > maxDistanceInKms) ? maxDistanceInKms : value;
            }
        }
    }
}
