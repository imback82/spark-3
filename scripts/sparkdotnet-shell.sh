#!/bin/bash

function spark_home_error() {
	echo "[sparkdotnet-shell.sh] Error - SPARK_HOME environment variable is not export"
	exit 1
}

function spark_major_minor_version_error() {
	echo "[sparkdotnet-shell.sh] Error - SPARK_MAJOR_MINOR_VERSION environment variable is not export"
	echo "[sparkdotnet-shell.sh] It should be set to 2.3 or 2.4, for example."
	exit 1
}

function dotnet_spark_version_error() {
	echo "[sparkdotnet-shell.sh] Error - DOTNET_SPARK_VERSION environment variable is not export"
	echo "[sparkdotnet-shell.sh] It should be set to the version of Microsoft.Spark (0.5.0, for example)."
	exit 1
}

function dotnet_spark_configuration() {
	echo "[sparkdotnet-shell.sh] Error - DOTNET_SPARK_CONFIGURATION environment variable is not export"
	echo "[sparkdotnet-shell.sh] It should be set to the build configration of Microsoft.Spark.REPL.dll (Debug/Release)."
	exit 1
}

SHELL_DIR="$( cd "$(dirname "$0")" ; pwd -P )"

[ "$SPARK_HOME" = "" ] && spark_home_error
[ "$SPARK_MAJOR_MINOR_VERSION" = "" ] && spark_major_minor_version_error
[ "$DOTNET_SPARK_VERSION" = "" ] && dotnet_spark_version_error
[ "$DOTNET_SPARK_CONFIGURATION" = "" ] && dotnet_spark_configuration 

echo $SHELL_DIR

$SPARK_HOME/bin/spark-submit --master local --class org.apache.spark.deploy.dotnet.DotnetRunner $SHELL_DIR/../src/scala/microsoft-spark-$SPARK_MAJOR_MINOR_VERSION.x/target/microsoft-spark-$SPARK_MAJOR_MINOR_VERSION.x-$DOTNET_SPARK_VERSION.jar dotnet $SHELL_DIR/../artifacts/bin/Microsoft.Spark.REPL/$DOTNET_SPARK_CONFIGURATION/netcoreapp2.1/Microsoft.Spark.REPL.dll
