using System.ComponentModel.DataAnnotations;
using LibraryApplication.Interfaces;

namespace LibraryApplication.Models;

public class BaseModel: IModelConcurrency
{
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}