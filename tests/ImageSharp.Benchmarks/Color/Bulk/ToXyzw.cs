﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace SixLabors.ImageSharp.Benchmarks.Color.Bulk
{
    using BenchmarkDotNet.Attributes;

    using SixLabors.ImageSharp.Memory;
    using SixLabors.ImageSharp.PixelFormats;

    public abstract class ToXyzw<TPixel>
        where TPixel : struct, IPixel<TPixel>
    {
        private Buffer<TPixel> source;

        private Buffer<byte> destination;

        [Params(16, 128, 1024)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.source = new Buffer<TPixel>(this.Count);
            this.destination = new Buffer<byte>(this.Count * 4);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.source.Dispose();
            this.destination.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void PerElement()
        {
            TPixel[] s = this.source.Array;
            byte[] d = this.destination.Array;
            var rgba = default(Rgba32);

            for (int i = 0; i < this.Count; i++)
            {
                TPixel c = s[i];
                int i4 = i * 4;
                c.ToRgba32(ref rgba);
                d[i4] = rgba.R;
                d[i4 + 1] = rgba.G;
                d[i4 + 2] = rgba.B;
                d[i4 + 3] = rgba.A;
            }
        }

        [Benchmark]
        public void CommonBulk()
        {
            new PixelOperations<TPixel>().ToRgba32Bytes(this.source, this.destination, this.Count);
        }

        [Benchmark]
        public void OptimizedBulk()
        {
            PixelOperations<TPixel>.Instance.ToRgba32Bytes(this.source, this.destination, this.Count);
        }
    }

    public class ToXyzw_Rgba32 : ToXyzw<Rgba32>
    {
    }

    public class ToXyzw_Argb32 : ToXyzw<Argb32>
    {
    }
}
