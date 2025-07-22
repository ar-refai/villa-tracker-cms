using System;

namespace VillaManager.Domain.DTOs;

public class PagginationDTO
{
    public int? Offset { get; set;} = 0;
    public int? Limit { get; set;} = 10;

}
