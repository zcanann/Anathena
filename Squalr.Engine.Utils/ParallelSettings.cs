namespace Squalr.Engine.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ParallelSettings
    {
        /// <summary>
        /// Settings that control the degree of parallelism for multithreaded tasks.
        /// </summary>
        private static Lazy<ParallelOptions> parallelSettingsFullCpu = new Lazy<ParallelOptions>(
                () =>
                {
                    ParallelOptions parallelOptions = new ParallelOptions()
                    {
                        // Full throttle; all processors used
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    };
                    return parallelOptions;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Settings that control the degree of parallelism for multithreaded tasks.
        /// </summary>
        private static Lazy<ParallelOptions> parallelSettingsFast = new Lazy<ParallelOptions>(
                () =>
                {
                    ParallelOptions parallelOptions = new ParallelOptions()
                    {
                        // Only use 75% of available processing power, as not to interfere with other programs
                        MaxDegreeOfParallelism = (Environment.ProcessorCount * 3) / 4
                    };
                    return parallelOptions;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Settings that control the degree of parallelism for multithreaded tasks.
        /// </summary>
        private static Lazy<ParallelOptions> parallelSettingsMedium = new Lazy<ParallelOptions>(
                () =>
                {
                    ParallelOptions parallelOptions = new ParallelOptions()
                    {
                        // Only use 25% of available processing power
                        MaxDegreeOfParallelism = (Environment.ProcessorCount * 1) / 4
                    };
                    return parallelOptions;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Settings that control the degree of parallelism for multithreaded tasks.
        /// </summary>
        private static Lazy<ParallelOptions> parallelSettingsNone = new Lazy<ParallelOptions>(
                () =>
                {
                    ParallelOptions parallelOptions = new ParallelOptions()
                    {
                        // Only use 1 CPU
                        MaxDegreeOfParallelism = 1
                    };
                    return parallelOptions;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Gets the parallelism settings which use all CPUs available.
        /// </summary>
        public static ParallelOptions ParallelSettingsFastest
        {
            get
            {
                return parallelSettingsFullCpu.Value;
            }
        }

        /// <summary>
        /// Gets the parallelism settings which use most of the CPUs available.
        /// </summary>
        public static ParallelOptions ParallelSettingsFast
        {
            get
            {
                return parallelSettingsFast.Value;
            }
        }

        /// <summary>
        /// Gets the parallelism settings which use some of the CPUs available.
        /// </summary>
        public static ParallelOptions ParallelSettingsMedium
        {
            get
            {
                return parallelSettingsMedium.Value;
            }
        }

        /// <summary>
        /// Gets the parallelism settings which use only one CPU. This should only be used for debugging.
        /// </summary>
        public static ParallelOptions ParallelSettingsNone
        {
            get
            {
                return parallelSettingsNone.Value;
            }
        }

        public static ParallelOptions Clone(this ParallelOptions parallelOptions)
        {
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism;

            return options;
        }
    }
    //// End class
}
//// End namespace