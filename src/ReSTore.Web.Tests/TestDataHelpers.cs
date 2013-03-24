using System;
using System.Collections.Generic;
using ReSTore.Web.Models;

namespace ReSTore.Web.Tests
{
    public static class TestDataHelpers
    {
        public static Guid _Area1Id = new Guid("B4460F44-AD88-47AE-B9DE-2D4A45A43D50");
        public static Guid _Area2Id = new Guid("37B0867D-DE11-4016-B703-05A41DD16542");
        public static Guid _Area3Id = new Guid("01910205-10A0-4833-99F2-E49DFAAA8D49");

        public static Guid _Category1Id = new Guid("5C574EBA-5A4C-4E2F-A819-C64DAD47EE5B");
        public static Guid _Category2Id = new Guid("7ABCF9A5-3BAB-4C6A-8731-5B344F5D7CC1");
        public static Guid _Category3Id = new Guid("81BD8EC2-802B-4A73-95CC-24316C6AD050");
        public static Guid _Category4Id = new Guid("C1892C06-9175-4C44-A66D-BF6FFC8B1FD4");
        public static Guid _Category5Id = new Guid("77C58F2B-7BD5-457E-A2C1-E982EAAA102E");
        public static Guid _Category6Id = new Guid("C672327E-3BAE-4295-872E-6C5B65F37CE0");

        public static Guid _Product1Id = new Guid("8E4A359A-B3DE-4B72-B27E-A7E27D3C6A0F");
        public static Guid _Product2Id = new Guid("15D0CA05-7D6E-4CFB-B737-850CAD2A5E1B");
        public static Guid _Product3Id = new Guid("4D29AADC-706B-4256-B387-9C6A0577169F");
        public static Guid _Product4Id = new Guid("A5F57D45-EB7E-4B9C-B923-BB3CE1634BD7");
        public static Guid _Product5Id = new Guid("5330DE43-C376-4DCF-8915-342E78835935");
        public static Guid _Product6Id = new Guid("AAC13562-4DD0-48FF-B290-E9F5D13E8BD1");
        public static Guid _Product7Id = new Guid("F0CD19AD-75EE-411D-AB14-4A812339F564");
        public static Guid _Product8Id = new Guid("03D332F9-6958-4FB5-91A4-3422AE069F2E");
        public static Guid _Product9Id = new Guid("0391BD5C-B113-4E3A-A2C2-F92CBB948D16");


        public static IEnumerable<Area> GetAreas()
        {
            yield return new Area() { Id = _Area1Id, Name = "Area 1", ImageHref = "/images/area/1.jpg" };
            yield return new Area() { Id = _Area2Id, Name = "Area 2", ImageHref = "/images/area/2.jpg" };
            yield return new Area() { Id = _Area3Id, Name = "Area 3", ImageHref = "/images/area/3.jpg" };
        }

        public static IEnumerable<Category> GetCategories()
        {
            yield return new Category() { Id = _Category1Id, AreaId = _Area1Id, Name="Category 1", ImageHref="/images/category/1" };
            yield return new Category() { Id = _Category2Id, AreaId = _Area1Id, Name="Category 2", ImageHref="/images/category/2" };
            yield return new Category() { Id = _Category3Id, AreaId = _Area2Id, Name="Category 3", ImageHref="/images/category/3" };
            yield return new Category() { Id = _Category4Id, AreaId = _Area3Id, Name="Category 4", ImageHref="/images/category/4" };
            yield return new Category() { Id = _Category5Id, AreaId = _Area3Id, Name="Category 5", ImageHref="/images/category/5" };
            yield return new Category() { Id = _Category6Id, AreaId = _Area3Id, Name="Category 6", ImageHref="/images/category/6" };
        }

        public static IEnumerable<Product> GetProducts()
        {
            yield return new Product() { Id = _Product1Id, CategoryId = _Category1Id, Name = "Product 1", Amount = "100 g", Brand = "Brand 1", ImageHref = "/images/product/1", Price = 101.50m };
            yield return new Product() { Id = _Product2Id, CategoryId = _Category1Id, Name = "Product 2", Amount = "200 g", Brand = "Brand 1", ImageHref = "/images/product/2", Price = 102.50m };
            yield return new Product() { Id = _Product3Id, CategoryId = _Category1Id, Name = "Product 3", Amount = "300 g", Brand = "Brand 2", ImageHref = "/images/product/3", Price = 103.50m };
            yield return new Product() { Id = _Product4Id, CategoryId = _Category1Id, Name = "Product 4", Amount = "400 g", Brand = "Brand 3", ImageHref = "/images/product/4", Price = 104.50m };
            yield return new Product() { Id = _Product5Id, CategoryId = _Category2Id, Name = "Product 5", Amount = "500 g", Brand = "Brand 1", ImageHref = "/images/product/5", Price = 105.50m };
            yield return new Product() { Id = _Product6Id, CategoryId = _Category2Id, Name = "Product 6", Amount = "600 g", Brand = "Brand 2", ImageHref = "/images/product/6", Price = 106.50m };
            yield return new Product() { Id = _Product7Id, CategoryId = _Category3Id, Name = "Product 7", Amount = "700 g", Brand = "Brand 3", ImageHref = "/images/product/7", Price = 107.50m };
            yield return new Product() { Id = _Product8Id, CategoryId = _Category4Id, Name = "Product 8", Amount = "800 g", Brand = "Brand 1", ImageHref = "/images/product/8", Price = 108.50m };
            yield return new Product() { Id = _Product9Id, CategoryId = _Category5Id, Name = "Product 9", Amount = "900 g", Brand = "Brand 2", ImageHref = "/images/product/9", Price = 109.50m };
        }
    }
}