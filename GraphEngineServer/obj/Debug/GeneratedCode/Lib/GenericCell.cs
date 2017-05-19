#pragma warning disable 162,168,649,660,661,1522

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace GraphEngineServer
{
    /// <summary>
    /// Exposes Load/Save/New operations of <see cref="Trinity.Storage.ICell"/> and Use operation on <see cref="Trinity.Storage.ICellAccessor"/> on <see cref="Trinity.Storage.LocalMemoryStorage"/> and <see cref="Trinity.Storage.MemoryCloud"/>.
    /// </summary>
    internal class GenericCellOperations : IGenericCellOperations
    {
        #region LocalMemoryStorage Save operations
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.Movie:
                    storage.SaveMovie((Movie)cell);
                    break;
                
                case CellType.Celebrity:
                    storage.SaveCelebrity((Celebrity)cell);
                    break;
                
            }
        }
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.Movie:
                    storage.SaveMovie(writeAheadLogOptions, (Movie)cell);
                    break;
                
                case CellType.Celebrity:
                    storage.SaveCelebrity(writeAheadLogOptions, (Celebrity)cell);
                    break;
                
            }
        }
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.Movie:
                    storage.SaveMovie(cellId, (Movie)cell);
                    break;
                
                case CellType.Celebrity:
                    storage.SaveCelebrity(cellId, (Celebrity)cell);
                    break;
                
            }
        }
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.Movie:
                    storage.SaveMovie(writeAheadLogOptions, cellId, (Movie)cell);
                    break;
                
                case CellType.Celebrity:
                    storage.SaveCelebrity(writeAheadLogOptions, cellId, (Celebrity)cell);
                    break;
                
            }
        }
        #endregion
        #region LocalMemoryStorage Load operations
        public unsafe ICell LoadGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            ushort type;
            int    size;
            byte*  cellPtr;
            int    entryIndex;
            var err = storage.GetLockedCellInfo(cellId, out size, out type, out cellPtr, out entryIndex);
            if (err != TrinityErrorCode.E_SUCCESS)
            {
                throw new CellNotFoundException("Cannot access the cell.");
            }
            switch ((CellType)type)
            {
                
                case CellType.Movie:
                    var Movie_accessor = new Movie_Accessor(cellPtr);
                    var Movie_cell = (Movie)Movie_accessor;
                    storage.ReleaseCellLock(cellId, entryIndex);
                    Movie_cell.CellID = cellId;
                    return Movie_cell;
                    break;
                
                case CellType.Celebrity:
                    var Celebrity_accessor = new Celebrity_Accessor(cellPtr);
                    var Celebrity_cell = (Celebrity)Celebrity_accessor;
                    storage.ReleaseCellLock(cellId, entryIndex);
                    Celebrity_cell.CellID = cellId;
                    return Celebrity_cell;
                    break;
                
                default:
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }
        #endregion
        #region New operations
        public ICell NewGenericCell(string cellType)
        {
            CellType type;
            if (!StorageSchema.cellTypeLookupTable.TryGetValue(cellType, out type))
                Throw.invalid_cell_type();
            switch (type)
            {
                
                case global::GraphEngineServer.CellType.Movie:
                    return new Movie();
                    break;
                
                case global::GraphEngineServer.CellType.Celebrity:
                    return new Celebrity();
                    break;
                
            }
            /* Should not reach here */
            return null;
        }
        public ICell NewGenericCell(long cellId, string cellType)
        {
            CellType type;
            if (!StorageSchema.cellTypeLookupTable.TryGetValue(cellType, out type))
                Throw.invalid_cell_type();
            switch (type)
            {
                
                case global::GraphEngineServer.CellType.Movie:
                    return new Movie(cell_id: cellId);
                    break;
                
                case global::GraphEngineServer.CellType.Celebrity:
                    return new Celebrity(cell_id: cellId);
                    break;
                
            }
            /* Should not reach here */
            return null;
        }
        public ICell NewGenericCell(string cellType, string content)
        {
            CellType type;
            if (!StorageSchema.cellTypeLookupTable.TryGetValue(cellType, out type))
                Throw.invalid_cell_type();
            switch (type)
            {
                
                case global::GraphEngineServer.CellType.Movie:
                    return Movie.Parse(content);
                    break;
                
                case global::GraphEngineServer.CellType.Celebrity:
                    return Celebrity.Parse(content);
                    break;
                
            }
            /* Should not reach here */
            return null;
        }
        #endregion
        #region LocalMemoryStorage Use operations
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long CellId)
        {
            ushort type;
            int    size;
            byte*  cellPtr;
            int    entryIndex;
            var err = storage.GetLockedCellInfo(CellId, out size, out type, out cellPtr, out entryIndex);
            if (err != TrinityErrorCode.E_SUCCESS)
            {
                throw new CellNotFoundException("Cannot access the cell.");
            }
            switch ((CellType)type)
            {
                
                case CellType.Movie:
                    return Movie_Accessor.New(CellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound);
                
                case CellType.Celebrity:
                    return Celebrity_Accessor.New(CellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound);
                
                default:
                    storage.ReleaseCellLock(CellId, entryIndex);
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
             }
        }
        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="GraphEngineServer.GenericCellAccessor"/> instance.</returns>
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long CellId, CellAccessOptions options)
        {
            ushort type;
            int    size;
            byte*  cellPtr;
            int    entryIndex;
            var err = storage.GetLockedCellInfo(CellId, out size, out type, out cellPtr, out entryIndex);
            switch (err)
            {
                case TrinityErrorCode.E_SUCCESS:
                    break;
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                    {
                        if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                        {
                            Throw.cell_not_found(CellId);
                        }
                        else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                        {
                            throw new ArgumentException("CellAccessOptions.CreateNewOnCellNotFound is not valid for UseGenericCell. Cannot determine new cell type.", "options");
                        }
                        else if ((options & CellAccessOptions.ReturnNullOnCellNotFound) != 0)
                        {
                            return null;
                        }
                        else
                        {
                            Throw.cell_not_found(CellId);
                        }
                        break;
                    }
                default:
                    throw new CellNotFoundException("Cannot access the cell.");
            }
            switch ((CellType)type)
            {
                
                case CellType.Movie:
                    return Movie_Accessor.New(CellId, cellPtr, entryIndex, options);
                
                case CellType.Celebrity:
                    return Celebrity_Accessor.New(CellId, cellPtr, entryIndex, options);
                
                default:
                    storage.ReleaseCellLock(CellId, entryIndex);
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
             };
        }
        #endregion
        #region LocalMemoryStorage Enumerate operations
        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    
                    case CellType.Movie:
                        {
                            var Movie_accessor = Movie_Accessor.AllocIterativeAccessor(cellInfo);
                            var Movie_cell     = (Movie)Movie_accessor;
                            Movie_accessor.Dispose();
                            yield return Movie_cell;
                            break;
                        }
                    
                    case CellType.Celebrity:
                        {
                            var Celebrity_accessor = Celebrity_Accessor.AllocIterativeAccessor(cellInfo);
                            var Celebrity_cell     = (Celebrity)Celebrity_accessor;
                            Celebrity_accessor.Dispose();
                            yield return Celebrity_cell;
                            break;
                        }
                    
                    default:
                        continue;
                }
            }
            yield break;
        }
        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    
                    case CellType.Movie:
                        {
                            var Movie_accessor = Movie_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return Movie_accessor;
                            Movie_accessor.Dispose();
                            break;
                        }
                    
                    case CellType.Celebrity:
                        {
                            var Celebrity_accessor = Celebrity_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return Celebrity_accessor;
                            Celebrity_accessor.Dispose();
                            break;
                        }
                    
                    default:
                        continue;
                }
            }
            yield break;
        }
        #endregion
        #region MemoryCloud operations
        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(Trinity.Storage.MemoryCloud storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.Movie:
                    storage.SaveMovie((Movie)cell);
                    break;
                
                case CellType.Celebrity:
                    storage.SaveCelebrity((Celebrity)cell);
                    break;
                
            }
        }
        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns></returns>
        public unsafe ICell LoadGenericCell(Trinity.Storage.MemoryCloud storage, long cellId)
        {
            ushort type;
            byte[] buff;
            var err = storage.LoadCell(cellId, out buff, out type);
            if (err != TrinityErrorCode.E_SUCCESS)
            {
                switch (err)
                {
                    case TrinityErrorCode.E_CELL_NOT_FOUND:
                        throw new CellNotFoundException("Cannot access the cell.");
                    case TrinityErrorCode.E_NETWORK_SEND_FAILURE:
                        throw new System.IO.IOException("Network error while accessing the cell.");
                    default:
                        throw new Exception("Cannot access the cell. Error code: " + err.ToString());
                }
            }
            switch ((CellType)type)
            {
                
                case CellType.Movie:
                    fixed (byte* Movie_ptr = buff)
                    {
                        Movie_Accessor/*_*/Movie_accessor = new Movie_Accessor(Movie_ptr);
                        Movie_accessor.CellID = cellId;
                        return (Movie)Movie_accessor;
                    }
                    break;
                
                case CellType.Celebrity:
                    fixed (byte* Celebrity_ptr = buff)
                    {
                        Celebrity_Accessor/*_*/Celebrity_accessor = new Celebrity_Accessor(Celebrity_ptr);
                        Celebrity_accessor.CellID = cellId;
                        return (Celebrity)Celebrity_accessor;
                    }
                    break;
                
                default:
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522
