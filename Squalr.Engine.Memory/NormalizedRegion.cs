namespace Squalr.Engine.Memory
{
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines an OS independent region in process memory space.
    /// </summary>
    public class NormalizedRegion
    {
        /// <summary>
        /// The size of the region.
        /// </summary>
        private Int32 regionSize;

        /// <summary>
        /// The memory alignment of this region.
        /// </summary>
        private Int32 alignment;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalizedRegion" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address of the region.</param>
        /// <param name="regionSize">The size of the region.</param>
        public NormalizedRegion(UInt64 baseAddress, Int32 regionSize)
        {
            this.Alignment = 1;
            this.BaseAddress = baseAddress;
            this.RegionSize = regionSize;
        }

        /// <summary>
        /// Gets or sets the base address of the region.
        /// </summary>
        public UInt64 BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the size of the region.
        /// </summary>
        public Int32 RegionSize
        {
            get
            {
                return this.regionSize;
            }

            set
            {
                this.regionSize = value;
            }
        }

        /// <summary>
        /// Gets the number of elements contained by this region.
        /// </summary>
        /// <returns>The number of elements contained by this region.</returns>
        public Int32 ElementCount
        {
            get
            {
                return this.RegionSize / this.Alignment;
            }
        }

        /// <summary>
        /// Gets or sets the end address of the region.
        /// </summary>
        public UInt64 EndAddress
        {
            get
            {
                return this.BaseAddress.Add(this.RegionSize);
            }

            set
            {
                this.RegionSize = value.Subtract(this.BaseAddress, wrapAround: false).ToInt32();
            }
        }

        /// <summary>
        /// Gets or sets the memory alignment, typically aligned with external process pointer size.
        /// </summary>
        public Int32 Alignment
        {
            get
            {
                return this.alignment;
            }

            set
            {
                this.alignment = value.Clamp(1, Int32.MaxValue);

                if (this.BaseAddress.Mod(this.alignment) == 0)
                {
                    return;
                }

                // Enforce alignment constraint on base address
                unchecked
                {
                    UInt64 endAddress = this.EndAddress;
                    this.BaseAddress = this.BaseAddress.Subtract(this.BaseAddress.Mod(this.alignment), wrapAround: false);
                    this.BaseAddress = this.BaseAddress.Add(this.alignment);
                    this.EndAddress = endAddress;
                }
            }
        }

        /// <summary>
        /// Determines if a page has a higher base address.
        /// </summary>
        /// <param name="first">The first region being compared.</param>
        /// <param name="second">The second region being compared.</param>
        /// <returns>The result of the comparison.</returns>
        public static Boolean operator >(NormalizedRegion first, NormalizedRegion second)
        {
            return first.BaseAddress > second.BaseAddress;
        }

        /// <summary>
        /// Determines if a page has a lower base address.
        /// </summary>
        /// <param name="first">The first region being compared.</param>
        /// <param name="second">The second region being compared.</param>
        /// <returns>The result of the comparison.</returns>
        public static Boolean operator <(NormalizedRegion first, NormalizedRegion second)
        {
            return first.BaseAddress < second.BaseAddress;
        }

        /// <summary>
        /// Determines if a page has an equal or higher base address.
        /// </summary>
        /// <param name="first">The first region being compared.</param>
        /// <param name="second">The second region being compared.</param>
        /// <returns>The result of the comparison.</returns>
        public static Boolean operator >=(NormalizedRegion first, NormalizedRegion second)
        {
            return first.BaseAddress >= second.BaseAddress;
        }

        /// <summary>
        /// Determines if a page has an equal or lower base address.
        /// </summary>
        /// <param name="first">The first region being compared.</param>
        /// <param name="second">The second region being compared.</param>
        /// <returns>The result of the comparison.</returns>
        public static Boolean operator <=(NormalizedRegion first, NormalizedRegion second)
        {
            return first.BaseAddress <= second.BaseAddress;
        }

        /// <summary>
        /// Determines if an address is contained in this snapshot.
        /// </summary>
        /// <param name="address">The address for which to search.</param>
        /// <returns>True if the address is contained.</returns>
        public virtual Boolean ContainsAddress(UInt64 address)
        {
            if (address >= this.BaseAddress && address <= this.EndAddress)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Expands a region by the element type size in both directions unconditionally.
        /// </summary>
        /// <param name="expandSize">The size by which to expand this region.</param>
        public virtual void Expand(Int32 expandSize)
        {
            this.BaseAddress = this.BaseAddress.Subtract(expandSize, wrapAround: false);
            this.RegionSize += expandSize * 2;
        }

        /// <summary>
        /// Returns a collection of regions within this region, based on the specified chunking size.
        /// Ex) If this region is 257 bytes, chunking with a size of 64 will return 5 new regions.
        /// </summary>
        /// <param name="chunkSize">The size to break down the region into.</param>
        /// <returns>A collection of regions broken down from the original region based on the chunk size.</returns>
        public IEnumerable<NormalizedRegion> ChunkNormalizedRegion(Int32 chunkSize)
        {
            if (chunkSize <= 0)
            {
                Logger.Log(LogLevel.Fatal, "Invalid chunk size specified for region");
                yield break;
            }

            chunkSize = Math.Min(chunkSize, this.RegionSize);

            Int32 chunkCount = (this.RegionSize / chunkSize) + (this.RegionSize % chunkSize == 0 ? 0 : 1);

            for (Int32 index = 0; index < chunkCount; index++)
            {
                Int32 size = chunkSize;

                // Set size to the remainder if on the final chunk and they are not divisible evenly
                if (index == chunkCount - 1 && this.RegionSize > chunkSize && this.RegionSize % chunkSize != 0)
                {
                    size = this.RegionSize % chunkSize;
                }

                yield return new NormalizedRegion(this.BaseAddress.Add(chunkSize * index), size);
            }
        }

        public static IEnumerable<NormalizedRegion> MergeAndSortRegions(IEnumerable<NormalizedRegion> regions)
        {
            if (regions == null || regions.Count() <= 0)
            {
                return null;
            }

            // First, sort by start address
            IList<NormalizedRegion> sortedRegions = regions.OrderBy(x => x.BaseAddress).ToList();

            // Create and initialize the stack with the first region
            Stack<NormalizedRegion> combinedRegions = new Stack<NormalizedRegion>();
            combinedRegions.Push(sortedRegions[0]);

            // Build the remaining regions
            for (Int32 index = combinedRegions.Count; index < sortedRegions.Count; index++)
            {
                NormalizedRegion top = combinedRegions.Peek();

                if (top.EndAddress < sortedRegions[index].BaseAddress)
                {
                    // If the interval does not overlap, put it on the top of the stack
                    combinedRegions.Push(sortedRegions[index]);
                }
                else if (top.EndAddress == sortedRegions[index].BaseAddress)
                {
                    // The regions are adjacent; merge them
                    top.RegionSize = sortedRegions[index].EndAddress.Subtract(top.BaseAddress).ToInt32();
                }
                else if (top.EndAddress <= sortedRegions[index].EndAddress)
                {
                    // The regions overlap
                    top.RegionSize = sortedRegions[index].EndAddress.Subtract(top.BaseAddress).ToInt32();
                }
            }

            return combinedRegions.ToList().OrderBy(x => x.BaseAddress).ToList();
        }
    }
    //// End interface
}
//// End namespace