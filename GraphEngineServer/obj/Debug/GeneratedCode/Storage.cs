
#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Runtime.CompilerServices;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.TSL.Lib;
using Trinity.Network;
using Trinity.Network.Sockets;
using Trinity.Network.Messaging;
using Trinity.TSL;
namespace GraphEngineServer
{

    ///<summary>
    ///Provides interfaces for accessing Movie cells
    ///on <see cref="Trinity.Storage.LocalMemorySotrage"/>.
    static public class StorageExtension_Movie
    {
		
        /// <summary>
        /// Adds a new cell of type Movie to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.LocalMemoryStorage storage,long CellID, string Key=null,string KGId=null,List<string> Genres=null,List<long> Artists=null,List<long> Directors=null,string Characters=null,List<string> Performance=null,string Distributors=null,string Channels=null,string Albums=null,string Name=null,string Alias=null,string Description=null,string Segments=null,string Categories=null,string IntEmbeddedFilters=null,string NumberOfWantToWatch=null,string Rating=null,string NumberOfShortReview=null,string ReviewCount=null,string NumberOfWatched=null,string NumberOfReviewer=null,string PublishDate=null,string Length=null,string Country=null,string Language=null,string SourceUrls=null,string ImageUrls=null,string OfficialSite=null,string EntityContainer=null,string Logo=null,string QueryRank=null)
		{

        byte* targetPtr = null;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

if(Artists!= null)
{
    targetPtr += Artists.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Directors!= null)
{
    targetPtr += Directors.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Key)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = KGId)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Genres[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

if(Artists!= null)
{
    *(int*)targetPtr = Artists.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Artists.Count;++iterator_0)
    {
*(long*)targetPtr = Artists[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Directors!= null)
{
    *(int*)targetPtr = Directors.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Directors.Count;++iterator_0)
    {
*(long*)targetPtr = Directors[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Characters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Performance[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Distributors)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Channels)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Albums)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Name)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Alias)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Description)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Segments)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Categories)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = IntEmbeddedFilters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWantToWatch)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Rating)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfShortReview)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ReviewCount)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWatched)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfReviewer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = PublishDate)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Length)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Country)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Language)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = SourceUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ImageUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = OfficialSite)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = EntityContainer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Logo)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = QueryRank)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }

        return storage.SaveCell(CellID, tmpcell, (ushort)CellType.Movie) == TrinityErrorCode.E_SUCCESS;
    }

        /// <summary>
        /// Adds a new cell of type Movie to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.LocalMemoryStorage storage, long CellID, Movie CellContent)
        {
            return SaveMovie(storage,CellID, CellContent.Key, CellContent.KGId, CellContent.Genres, CellContent.Artists, CellContent.Directors, CellContent.Characters, CellContent.Performance, CellContent.Distributors, CellContent.Channels, CellContent.Albums, CellContent.Name, CellContent.Alias, CellContent.Description, CellContent.Segments, CellContent.Categories, CellContent.IntEmbeddedFilters, CellContent.NumberOfWantToWatch, CellContent.Rating, CellContent.NumberOfShortReview, CellContent.ReviewCount, CellContent.NumberOfWatched, CellContent.NumberOfReviewer, CellContent.PublishDate, CellContent.Length, CellContent.Country, CellContent.Language, CellContent.SourceUrls, CellContent.ImageUrls, CellContent.OfficialSite, CellContent.EntityContainer, CellContent.Logo, CellContent.QueryRank);
        }
        /// <summary>
        /// Adds a new cell of type Movie to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.LocalMemoryStorage storage, Movie CellContent)
        {
            return SaveMovie(storage,CellContent.CellID, CellContent.Key, CellContent.KGId, CellContent.Genres, CellContent.Artists, CellContent.Directors, CellContent.Characters, CellContent.Performance, CellContent.Distributors, CellContent.Channels, CellContent.Albums, CellContent.Name, CellContent.Alias, CellContent.Description, CellContent.Segments, CellContent.Categories, CellContent.IntEmbeddedFilters, CellContent.NumberOfWantToWatch, CellContent.Rating, CellContent.NumberOfShortReview, CellContent.ReviewCount, CellContent.NumberOfWatched, CellContent.NumberOfReviewer, CellContent.PublishDate, CellContent.Length, CellContent.Country, CellContent.Language, CellContent.SourceUrls, CellContent.ImageUrls, CellContent.OfficialSite, CellContent.EntityContainer, CellContent.Logo, CellContent.QueryRank);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a Movie. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="GraphEngineServer.Movie"/> instance.</returns>
        public unsafe static Movie_Accessor UseMovie(this Trinity.Storage.LocalMemoryStorage storage,long CellID, CellAccessOptions options)
        {

            return Movie_Accessor.New(CellID,options);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a Movie. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <returns>A <see cref="GraphEngineServer.Movie"/> instance.</returns>
        public unsafe static Movie_Accessor UseMovie(this Trinity.Storage.LocalMemoryStorage storage,long CellID)
        {

            return Movie_Accessor.New(CellID,CellAccessOptions.ThrowExceptionOnCellNotFound);
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static Movie LoadMovie(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            using (var cell = new Movie_Accessor(CellID, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                Movie ret = cell;
                ret.CellID = CellID;
                return ret;
            }
        }
		
        /// <summary>
        /// Adds a new cell of type Movie to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, string Key=null,string KGId=null,List<string> Genres=null,List<long> Artists=null,List<long> Directors=null,string Characters=null,List<string> Performance=null,string Distributors=null,string Channels=null,string Albums=null,string Name=null,string Alias=null,string Description=null,string Segments=null,string Categories=null,string IntEmbeddedFilters=null,string NumberOfWantToWatch=null,string Rating=null,string NumberOfShortReview=null,string ReviewCount=null,string NumberOfWatched=null,string NumberOfReviewer=null,string PublishDate=null,string Length=null,string Country=null,string Language=null,string SourceUrls=null,string ImageUrls=null,string OfficialSite=null,string EntityContainer=null,string Logo=null,string QueryRank=null)
		{

        byte* targetPtr = null;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

if(Artists!= null)
{
    targetPtr += Artists.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Directors!= null)
{
    targetPtr += Directors.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Key)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = KGId)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Genres[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

if(Artists!= null)
{
    *(int*)targetPtr = Artists.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Artists.Count;++iterator_0)
    {
*(long*)targetPtr = Artists[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Directors!= null)
{
    *(int*)targetPtr = Directors.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Directors.Count;++iterator_0)
    {
*(long*)targetPtr = Directors[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Characters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Performance[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Distributors)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Channels)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Albums)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Name)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Alias)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Description)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Segments)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Categories)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = IntEmbeddedFilters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWantToWatch)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Rating)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfShortReview)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ReviewCount)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWatched)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfReviewer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = PublishDate)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Length)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Country)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Language)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = SourceUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ImageUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = OfficialSite)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = EntityContainer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Logo)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = QueryRank)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }

        return storage.SaveCell(options, CellID, tmpcell, (ushort)CellType.Movie) == TrinityErrorCode.E_SUCCESS;
    }

        /// <summary>
        /// Adds a new cell of type Movie to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, Movie CellContent)
        {
            return SaveMovie(storage, options, CellID, CellContent.Key, CellContent.KGId, CellContent.Genres, CellContent.Artists, CellContent.Directors, CellContent.Characters, CellContent.Performance, CellContent.Distributors, CellContent.Channels, CellContent.Albums, CellContent.Name, CellContent.Alias, CellContent.Description, CellContent.Segments, CellContent.Categories, CellContent.IntEmbeddedFilters, CellContent.NumberOfWantToWatch, CellContent.Rating, CellContent.NumberOfShortReview, CellContent.ReviewCount, CellContent.NumberOfWatched, CellContent.NumberOfReviewer, CellContent.PublishDate, CellContent.Length, CellContent.Country, CellContent.Language, CellContent.SourceUrls, CellContent.ImageUrls, CellContent.OfficialSite, CellContent.EntityContainer, CellContent.Logo, CellContent.QueryRank);
        }
        /// <summary>
        /// Adds a new cell of type Movie to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, Movie CellContent)
        {
            return SaveMovie(storage, options, CellContent.CellID, CellContent.Key, CellContent.KGId, CellContent.Genres, CellContent.Artists, CellContent.Directors, CellContent.Characters, CellContent.Performance, CellContent.Distributors, CellContent.Channels, CellContent.Albums, CellContent.Name, CellContent.Alias, CellContent.Description, CellContent.Segments, CellContent.Categories, CellContent.IntEmbeddedFilters, CellContent.NumberOfWantToWatch, CellContent.Rating, CellContent.NumberOfShortReview, CellContent.ReviewCount, CellContent.NumberOfWatched, CellContent.NumberOfReviewer, CellContent.PublishDate, CellContent.Length, CellContent.Country, CellContent.Language, CellContent.SourceUrls, CellContent.ImageUrls, CellContent.OfficialSite, CellContent.EntityContainer, CellContent.Logo, CellContent.QueryRank);
        }		
        /// <summary>
        /// Adds a new cell of type Movie to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters. 
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.MemoryCloud storage,long CellID, string Key=null,string KGId=null,List<string> Genres=null,List<long> Artists=null,List<long> Directors=null,string Characters=null,List<string> Performance=null,string Distributors=null,string Channels=null,string Albums=null,string Name=null,string Alias=null,string Description=null,string Segments=null,string Categories=null,string IntEmbeddedFilters=null,string NumberOfWantToWatch=null,string Rating=null,string NumberOfShortReview=null,string ReviewCount=null,string NumberOfWatched=null,string NumberOfReviewer=null,string PublishDate=null,string Length=null,string Country=null,string Language=null,string SourceUrls=null,string ImageUrls=null,string OfficialSite=null,string EntityContainer=null,string Logo=null,string QueryRank=null)
		{

        byte* targetPtr = null;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

if(Artists!= null)
{
    targetPtr += Artists.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Directors!= null)
{
    targetPtr += Directors.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Key)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = KGId)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Genres[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

if(Artists!= null)
{
    *(int*)targetPtr = Artists.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Artists.Count;++iterator_0)
    {
*(long*)targetPtr = Artists[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Directors!= null)
{
    *(int*)targetPtr = Directors.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Directors.Count;++iterator_0)
    {
*(long*)targetPtr = Directors[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Characters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Performance[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Distributors)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Channels)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Albums)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Name)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Alias)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Description)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Segments)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Categories)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = IntEmbeddedFilters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWantToWatch)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Rating)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfShortReview)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ReviewCount)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWatched)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfReviewer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = PublishDate)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Length)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Country)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Language)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = SourceUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ImageUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = OfficialSite)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = EntityContainer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Logo)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = QueryRank)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }

            return storage.SaveCell(CellID, tmpcell,  (ushort)CellType.Movie) == TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// Adds a new cell of type Movie to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.MemoryCloud storage, long CellID, Movie CellContent)
        {
            return SaveMovie(storage,CellID, CellContent.Key, CellContent.KGId, CellContent.Genres, CellContent.Artists, CellContent.Directors, CellContent.Characters, CellContent.Performance, CellContent.Distributors, CellContent.Channels, CellContent.Albums, CellContent.Name, CellContent.Alias, CellContent.Description, CellContent.Segments, CellContent.Categories, CellContent.IntEmbeddedFilters, CellContent.NumberOfWantToWatch, CellContent.Rating, CellContent.NumberOfShortReview, CellContent.ReviewCount, CellContent.NumberOfWatched, CellContent.NumberOfReviewer, CellContent.PublishDate, CellContent.Length, CellContent.Country, CellContent.Language, CellContent.SourceUrls, CellContent.ImageUrls, CellContent.OfficialSite, CellContent.EntityContainer, CellContent.Logo, CellContent.QueryRank);
        }

        /// <summary>
        /// Adds a new cell of type Movie to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SaveMovie(this Trinity.Storage.MemoryCloud storage, Movie CellContent)
        {
            return SaveMovie(storage,CellContent.CellID, CellContent.Key, CellContent.KGId, CellContent.Genres, CellContent.Artists, CellContent.Directors, CellContent.Characters, CellContent.Performance, CellContent.Distributors, CellContent.Channels, CellContent.Albums, CellContent.Name, CellContent.Alias, CellContent.Description, CellContent.Segments, CellContent.Categories, CellContent.IntEmbeddedFilters, CellContent.NumberOfWantToWatch, CellContent.Rating, CellContent.NumberOfShortReview, CellContent.ReviewCount, CellContent.NumberOfWatched, CellContent.NumberOfReviewer, CellContent.PublishDate, CellContent.Length, CellContent.Country, CellContent.Language, CellContent.SourceUrls, CellContent.ImageUrls, CellContent.OfficialSite, CellContent.EntityContainer, CellContent.Logo, CellContent.QueryRank);
        }

        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static Movie LoadMovie(this Trinity.Storage.MemoryCloud storage, long CellID)
        {
            byte[] cellBuff;
            var eResult = storage.LoadCell(CellID, out cellBuff);
            if(eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            { throw new CellNotFoundException("Cell with ID=" + CellID + " not found!"); }
            else if(eResult == TrinityErrorCode.E_NETWORK_SEND_FAILURE)
            { throw new IOException("Network error when loading cell with ID=" + CellID); }
            fixed(byte* ptr = cellBuff)
            {
                using (var cell = new Movie_Accessor(ptr))
                {
                    Movie ret = cell;
                    ret.CellID = CellID;
                    return ret;
                }
            }
        }
}
    ///<summary>
    ///Provides interfaces for accessing Person cells
    ///on <see cref="Trinity.Storage.LocalMemorySotrage"/>.
    static public class StorageExtension_Person
    {
		
        /// <summary>
        /// Adds a new cell of type Person to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.LocalMemoryStorage storage,long CellID, int age=default(int),long parent=default(long),string Name=null,byte Gender=default(byte),bool Married=default(bool),long Spouse=default(long),List<long> Act=null,List<long> Direct=null)
		{

        byte* targetPtr = null;
targetPtr += 4;
targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
targetPtr += 1;
targetPtr += 1;
targetPtr += 8;

if(Act!= null)
{
    targetPtr += Act.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Direct!= null)
{
    targetPtr += Direct.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;
*(int*)targetPtr = age;
            targetPtr += 4;
*(long*)targetPtr = parent;
            targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Name)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
*(byte*)targetPtr = Gender;
            targetPtr += 1;
*(bool*)targetPtr = Married;
            targetPtr += 1;
*(long*)targetPtr = Spouse;
            targetPtr += 8;

if(Act!= null)
{
    *(int*)targetPtr = Act.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Act.Count;++iterator_0)
    {
*(long*)targetPtr = Act[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Direct!= null)
{
    *(int*)targetPtr = Direct.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Direct.Count;++iterator_0)
    {
*(long*)targetPtr = Direct[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        }

        return storage.SaveCell(CellID, tmpcell, (ushort)CellType.Person) == TrinityErrorCode.E_SUCCESS;
    }

        /// <summary>
        /// Adds a new cell of type Person to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.LocalMemoryStorage storage, long CellID, Person CellContent)
        {
            return SavePerson(storage,CellID, CellContent.age, CellContent.parent, CellContent.Name, CellContent.Gender, CellContent.Married, CellContent.Spouse, CellContent.Act, CellContent.Direct);
        }
        /// <summary>
        /// Adds a new cell of type Person to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.LocalMemoryStorage storage, Person CellContent)
        {
            return SavePerson(storage,CellContent.CellID, CellContent.age, CellContent.parent, CellContent.Name, CellContent.Gender, CellContent.Married, CellContent.Spouse, CellContent.Act, CellContent.Direct);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a Person. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="GraphEngineServer.Person"/> instance.</returns>
        public unsafe static Person_Accessor UsePerson(this Trinity.Storage.LocalMemoryStorage storage,long CellID, CellAccessOptions options)
        {

            return Person_Accessor.New(CellID,options);
        }
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a Person. Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <returns>A <see cref="GraphEngineServer.Person"/> instance.</returns>
        public unsafe static Person_Accessor UsePerson(this Trinity.Storage.LocalMemoryStorage storage,long CellID)
        {

            return Person_Accessor.New(CellID,CellAccessOptions.ThrowExceptionOnCellNotFound);
        }
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static Person LoadPerson(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            using (var cell = new Person_Accessor(CellID, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                Person ret = cell;
                ret.CellID = CellID;
                return ret;
            }
        }
		
        /// <summary>
        /// Adds a new cell of type Person to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, int age=default(int),long parent=default(long),string Name=null,byte Gender=default(byte),bool Married=default(bool),long Spouse=default(long),List<long> Act=null,List<long> Direct=null)
		{

        byte* targetPtr = null;
targetPtr += 4;
targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
targetPtr += 1;
targetPtr += 1;
targetPtr += 8;

if(Act!= null)
{
    targetPtr += Act.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Direct!= null)
{
    targetPtr += Direct.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;
*(int*)targetPtr = age;
            targetPtr += 4;
*(long*)targetPtr = parent;
            targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Name)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
*(byte*)targetPtr = Gender;
            targetPtr += 1;
*(bool*)targetPtr = Married;
            targetPtr += 1;
*(long*)targetPtr = Spouse;
            targetPtr += 8;

if(Act!= null)
{
    *(int*)targetPtr = Act.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Act.Count;++iterator_0)
    {
*(long*)targetPtr = Act[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Direct!= null)
{
    *(int*)targetPtr = Direct.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Direct.Count;++iterator_0)
    {
*(long*)targetPtr = Direct[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        }

        return storage.SaveCell(options, CellID, tmpcell, (ushort)CellType.Person) == TrinityErrorCode.E_SUCCESS;
    }

        /// <summary>
        /// Adds a new cell of type Person to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, Person CellContent)
        {
            return SavePerson(storage, options, CellID, CellContent.age, CellContent.parent, CellContent.Name, CellContent.Gender, CellContent.Married, CellContent.Spouse, CellContent.Act, CellContent.Direct);
        }
        /// <summary>
        /// Adds a new cell of type Person to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, Person CellContent)
        {
            return SavePerson(storage, options, CellContent.CellID, CellContent.age, CellContent.parent, CellContent.Name, CellContent.Gender, CellContent.Married, CellContent.Spouse, CellContent.Act, CellContent.Direct);
        }		
        /// <summary>
        /// Adds a new cell of type Person to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters. 
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.MemoryCloud storage,long CellID, int age=default(int),long parent=default(long),string Name=null,byte Gender=default(byte),bool Married=default(bool),long Spouse=default(long),List<long> Act=null,List<long> Direct=null)
		{

        byte* targetPtr = null;
targetPtr += 4;
targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
targetPtr += 1;
targetPtr += 1;
targetPtr += 8;

if(Act!= null)
{
    targetPtr += Act.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Direct!= null)
{
    targetPtr += Direct.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;
*(int*)targetPtr = age;
            targetPtr += 4;
*(long*)targetPtr = parent;
            targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Name)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
*(byte*)targetPtr = Gender;
            targetPtr += 1;
*(bool*)targetPtr = Married;
            targetPtr += 1;
*(long*)targetPtr = Spouse;
            targetPtr += 8;

if(Act!= null)
{
    *(int*)targetPtr = Act.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Act.Count;++iterator_0)
    {
*(long*)targetPtr = Act[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Direct!= null)
{
    *(int*)targetPtr = Direct.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Direct.Count;++iterator_0)
    {
*(long*)targetPtr = Direct[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        }

            return storage.SaveCell(CellID, tmpcell,  (ushort)CellType.Person) == TrinityErrorCode.E_SUCCESS;
        }

        /// <summary>
        /// Adds a new cell of type Person to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name="CellID"/> overrides the cell id in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellID">A 64-bit cell Id.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.MemoryCloud storage, long CellID, Person CellContent)
        {
            return SavePerson(storage,CellID, CellContent.age, CellContent.parent, CellContent.Name, CellContent.Gender, CellContent.Married, CellContent.Spouse, CellContent.Act, CellContent.Direct);
        }

        /// <summary>
        /// Adds a new cell of type Person to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="CellContent">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool SavePerson(this Trinity.Storage.MemoryCloud storage, Person CellContent)
        {
            return SavePerson(storage,CellContent.CellID, CellContent.age, CellContent.parent, CellContent.Name, CellContent.Gender, CellContent.Married, CellContent.Spouse, CellContent.Act, CellContent.Direct);
        }

        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static Person LoadPerson(this Trinity.Storage.MemoryCloud storage, long CellID)
        {
            byte[] cellBuff;
            var eResult = storage.LoadCell(CellID, out cellBuff);
            if(eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            { throw new CellNotFoundException("Cell with ID=" + CellID + " not found!"); }
            else if(eResult == TrinityErrorCode.E_NETWORK_SEND_FAILURE)
            { throw new IOException("Network error when loading cell with ID=" + CellID); }
            fixed(byte* ptr = cellBuff)
            {
                using (var cell = new Person_Accessor(ptr))
                {
                    Person ret = cell;
                    ret.CellID = CellID;
                    return ret;
                }
            }
        }
}
    /// <summary>
    /// Provides cell type interfaces on <see cref="Trinity.Storage.LocalMemoryStorage"/>.
    /// </summary>
    static public class Storage_CellType_Extension
    {
        /// <summary>
        /// Tells whether the cell with the given id is a Movie .
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">The id of the cell.</param>
        /// <returns>True if the cell is found and is of the correct type. Otherwise false.</returns>
        public unsafe static bool IsMovie(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            ushort cellType;
            if(storage.GetCellType(CellID, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return cellType == (ushort)CellType.Movie; 
            }
            return false;
        }
        /// <summary>
        /// Tells whether the cell with the given id is a Person .
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">The id of the cell.</param>
        /// <returns>True if the cell is found and is of the correct type. Otherwise false.</returns>
        public unsafe static bool IsPerson(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            ushort cellType;
            if(storage.GetCellType(CellID, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return cellType == (ushort)CellType.Person; 
            }
            return false;
        }
        /// <summary>
        /// Get the type of the cell.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">The id of the cell.</param>
        /// <returns>If the cell is found, returns the type of the cell. Otherwise, returns CellType.Undefined.</returns>
        public unsafe static CellType GetCellType(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            ushort cellType;
            if(storage.GetCellType(CellID, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return (CellType)cellType; 
            }
            else
            {
                return CellType.Undefined;
            }
        }
//        public unsafe static void Transform(this Trinity.Storage.LocalMemoryStorage storage, params CellConverter[] converters)
//        {
//            Dictionary<ushort, CellTransformAction<long,int,ushort>> converterMap = new Dictionary<ushort, CellTransformAction<long,int,ushort>>();
//            foreach (var converter in converters)
//            {
//                converterMap[(ushort)converter.SourceCellType] = converter._action;
//            }
//            storage.TransformCells((byte* ptr, long id,  int count, ref ushort cellType) =>
//                {
//                    //ushort cellType = *(ushort*)ptr;
//                    if (converterMap.ContainsKey(cellType))
//                    {
//                        return converterMap[cellType](ptr, id, count, ref cellType);
//                    }
//                    else
//                    {
//                        byte[] ret = new byte[count];
//                        Memory.Copy(ptr, 0, ret, 0, count);
//                        return ret;
//                    }
//                });
//        }
    }
}

#pragma warning restore 162,168,649,660,661,1522
