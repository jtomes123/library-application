using LibraryApplication.Models;

namespace LibraryApplication.Dtos;

public record HistoryEntryDto(Guid Id, DateTime TimeStamp, HistoryAction Action, Guid User);