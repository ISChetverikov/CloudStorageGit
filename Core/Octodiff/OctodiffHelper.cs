using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octodiff;
using Octodiff.Core;
using Octodiff.Diagnostics;

namespace Core.Octodiff
{
    public static class OctodiffHelper
    {

        public static void BuildSignature(string filePath, string signatureFilePath)
        {
            
            var signatureBuilder = new SignatureBuilder();
            using (var basisStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
            }
        }

        public static void CreateDelta(string filePath, string signatureFilePath, string deltaFilePath)
        {
            
            var deltaBuilder = new DeltaBuilder();
            using (var newFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var signatureFileStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, new ConsoleProgressReporter()), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
            }
        }
        
        public static void ApplyDelta(string filePath, string signaturePath, string deltaPath)
        {

            var deltaApplier = new DeltaApplier { SkipHashCheck = false };
            using (var basisStream = new FileStream(signaturePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var newFileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()), newFileStream);
            }
        }

        public static string GetSignatureName(string filePath)
        {
            return Path.GetFileName(filePath) + ".octosig";
        }

        public static string GetDeltaName(string filePath)
        {
            return Path.GetFileName(filePath) + ".octodelta";
        }
    }
}
