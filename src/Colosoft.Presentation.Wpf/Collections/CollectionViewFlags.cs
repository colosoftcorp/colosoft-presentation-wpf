using System;

namespace Colosoft.Collections
{
    [Flags]
#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
    internal enum CollectionViewFlags
#pragma warning restore S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
    {
        CachedIsEmpty = 0x200,
        IsCurrentAfterLast = 0x10,
        IsCurrentBeforeFirst = 8,
        IsDataInGroupOrder = 0x40,
        IsDynamic = 0x20,
        IsMultiThreadCollectionChangeAllowed = 0x100,
        NeedsRefresh = 0x80,
        ShouldProcessCollectionChanged = 4,
        UpdatedOutsideDispatcher = 2,
    }
}
