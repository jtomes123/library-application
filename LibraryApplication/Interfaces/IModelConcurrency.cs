using System.ComponentModel.DataAnnotations;

namespace LibraryApplication.Interfaces;

public interface IModelConcurrency
{
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}