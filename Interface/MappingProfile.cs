using AutoMapper;
using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Interface
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Example: Mapping between Product and ProductDto
            CreateMap<Product, ProductModel>().ReverseMap();
        }
    }
}