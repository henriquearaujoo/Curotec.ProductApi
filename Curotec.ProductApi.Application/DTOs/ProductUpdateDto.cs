﻿namespace Curotec.ProductApi.Application.DTOs;

public class ProductUpdateDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}