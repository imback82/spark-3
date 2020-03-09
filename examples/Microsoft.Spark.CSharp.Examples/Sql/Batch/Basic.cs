// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Types;
using static Microsoft.Spark.Sql.Functions;

namespace Microsoft.Spark.Examples.Sql.Batch
{
    /// <summary>
    /// A simple example demonstrating basic Spark SQL features.
    /// </summary>
    internal sealed class Basic : IExample
    {
        public void Run(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine(
                    "Usage: Basic <path to SPARK_HOME/examples/src/main/resources/people.json>");
                Environment.Exit(1);
            }

            SparkSession spark = SparkSession
                .Builder()
                .AppName(".NET Spark SQL basic example")
                .GetOrCreate();

            var df = spark
                .Read()
                .Schema(new StructType(new[]
                    {
                        new StructField("date", new DateType())
                    }))
                .Json(args[0]);
            df.PrintSchema();
            df.Show();

            spark.Stop();
        }
    }
}
