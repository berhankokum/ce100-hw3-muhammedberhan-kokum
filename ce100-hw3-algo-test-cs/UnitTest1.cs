using ce100_hw3_algo_lib_cs;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;


namespace ce100_hw3_algo_test_cs
{
    public class UnitTest1
    {
        [Fact]
        public void HuffmanCodingTest()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            /** @brief Define the path of the music file to be compressed.**/
            string musicFilePath = "Music.mp3";
            /** @brief Define the path of the text file to be compressed.**/
            string textFilePath = "Lorem ipsum.txt";
            /** @brief Generate a string of lorem ipsum text with the same size as the music file.**/
            string loremIpsum = GenerateLoremIpsum(FileSizeInBytes(musicFilePath));
            /** @brief Write the lorem ipsum text to the text file.**/
            File.WriteAllText(textFilePath, loremIpsum);
            /** @brief Create a new HuffmanCoding object.**/
            HuffmanCoding huffmanCoding = new HuffmanCoding();
            /** @brief Define the paths of the compressed music and text files.**/
            string compressedMusicFilePath = "Music.bin";
            string compressedTextFilePath = "Lorem ipsum.bin";
            /** @brief Compress the music and text files.**/
            huffmanCoding.CompressMusic(musicFilePath, compressedMusicFilePath);
            huffmanCoding.Compress(textFilePath, compressedTextFilePath);
            /** @brief Calculate the compression ratios of the music and text files.**/
            double musicCompressionRatio = (double)FileSizeInBytes(compressedMusicFilePath) / FileSizeInBytes(musicFilePath);
            double textCompressionRatio = (double)FileSizeInBytes(compressedTextFilePath) / FileSizeInBytes(textFilePath);
            /** @brief Output the compression ratios to the console.**/
            stopwatch.Stop();
            _output.WriteLine($"Music file compression rate: {musicCompressionRatio}");
            _output.WriteLine($"Text file compression rate: {textCompressionRatio}");
            /** @brief Define the paths of the decompressed music and text files.**/
            string decompressedMusicFilePath = "Decompressed Music.mp3";
            string decompressedTextFilePath = "Decompressed Lorem ipsum.txt";
            /** @brief Decompress the music and text files.**/
            huffmanCoding.DecompressMusic(compressedMusicFilePath, decompressedMusicFilePath);
            huffmanCoding.Decompress(compressedTextFilePath, decompressedTextFilePath);
            /** @brief Read all bytes of the original and decompressed music files.**/
            byte[] originalFileBytes = File.ReadAllBytes(musicFilePath);
            byte[] newFileBytes = File.ReadAllBytes(decompressedMusicFilePath);
            /** @brief Read all text of the original and decompressed text files.**/
            string originalFile1 = File.ReadAllText(textFilePath);
            string newFile1 = File.ReadAllText(decompressedTextFilePath);
            /** @brief Assert that the decompressed files are equal to the original files.**/
            Assert.Equal(originalFile1, newFile1);
            Assert.Equal(originalFileBytes, newFileBytes);
        }

        /** @brief This is a unit test method for testing the functionality of the IkeaAssemblyGuide class.**/
        [Fact]
        public void DFStest()
        {
            /** @brief Create a new instance of the IkeaAssemblyGuide class.**/
            IkeaAssemblyGuide guide = new IkeaAssemblyGuide();
            /** @brief The link to the assembly instructions for a Brimnes wardrobe.**/
            string link = "https://www.ikea.com/us/en/assembly_instructions/brimnes-wardrobe-with-2-doors-white__AA-2185045-1-2.pdf";
            /** @brief Add an assembly step to the guide.**/
            guide.AddAssemblyStep(1, "Check all parts. Verify that parts are complete and undamaged.", new List<int>());
            guide.AddAssemblyStep(2, "Screw the part 109220 to the side panels of the wardrobe with the screws no: 105248.", new List<int> { 1 });
            guide.AddAssemblyStep(3, "Install screw 118331 at the bottom of the side panel.", new List<int> { 2 });
            guide.AddAssemblyStep(4, "Install screw 101345 on the side parts of the bottom panel.", new List<int> { 3 });
            guide.AddAssemblyStep(5, "Join the bottom panel to the side panels using parts 119252 and 119250.", new List<int> { 4 });
            guide.AddAssemblyStep(6, "Assemble the rear panel of the lower shelves with screw number 101345.", new List<int> { 5 });
            guide.AddAssemblyStep(7, "Mount the back panel of the bottom shelf of the wardrobe to the side panels of the wardrobe.", new List<int> { 6 });
            guide.AddAssemblyStep(8, "In the same way assemble the shelves of the cabinet.", new List<int> { 7 });
            guide.AddAssemblyStep(9, "Mount the right panel of the wardrobe using screws 119252 119250 101345.", new List<int> { 8 });
            guide.AddAssemblyStep(10, "Attach screws 123840, 109837 and 118331 to the top panel of the cabinet.", new List<int> { 9 });
            guide.AddAssemblyStep(11, "Mount the bottom panel of the cabinet using screw 101345.", new List<int> { 10 });
            guide.AddAssemblyStep(12, "Assemble the back panel of the cabinet and draw the line where the shelves come from with a pencil.", new List<int> { 11 });
            guide.AddAssemblyStep(13, "Screw the back panel of the cabinet with screw 101206.", new List<int> { 12 });
            guide.AddAssemblyStep(14, "Assemble the cabinet with the help of a hammer.", new List<int> { 13 });
            guide.AddAssemblyStep(15, "Fix the cabinet to the wall with piece 106989.", new List<int> { 14 });
            guide.AddAssemblyStep(16, "Attach the part numbered 109336 to the doors of the cabinet with the screws numbered 105248.", new List<int> { 15 });
            guide.AddAssemblyStep(17, "Mount the cabinet doors to the cabinet with screw number 105248.", new List<int> { 16 });
            guide.AddAssemblyStep(18, "Mount the handle of the cabinet to the doors of the cabinet with the screw number 109567.", new List<int> { 17 });
            /** @brief Get the assembly steps in text format.**/
            List<string> assemblyStepsText = guide.GetAssemblyStepsText();
            /** @brief Assert that the number of assembly steps is 1.**/
            Assert.Equal(18, assemblyStepsText.Count);
            /** @brief Assert that the text of the assembly step matches the expected value.**/
            Assert.Equal("Step 1: Check all parts. Verify that parts are complete and undamaged.", assemblyStepsText[0]);
            Assert.Equal("Step 2: Screw the part 109220 to the side panels of the wardrobe with the screws no: 105248.", assemblyStepsText[1]);
            Assert.Equal("Step 3: Install screw 118331 at the bottom of the side panel.", assemblyStepsText[2]);
            Assert.Equal("Step 4: Install screw 101345 on the side parts of the bottom panel.", assemblyStepsText[3]);
            Assert.Equal("Step 5: Join the bottom panel to the side panels using parts 119252 and 119250.", assemblyStepsText[4]);
            Assert.Equal("Step 6: Assemble the rear panel of the lower shelves with screw number 101345.", assemblyStepsText[5]);
            Assert.Equal("Step 7: Mount the back panel of the bottom shelf of the wardrobe to the side panels of the wardrobe.", assemblyStepsText[6]);
            Assert.Equal("Step 8: In the same way assemble the shelves of the cabinet.", assemblyStepsText[7]);
            Assert.Equal("Step 9: Mount the right panel of the wardrobe using screws 119252 119250 101345.", assemblyStepsText[8]);
            Assert.Equal("Step 10: Attach screws 123840, 109837 and 118331 to the top panel of the cabinet.", assemblyStepsText[9]);
            Assert.Equal("Step 11: Mount the bottom panel of the cabinet using screw 101345.", assemblyStepsText[10]);
            Assert.Equal("Step 12: Assemble the back panel of the cabinet and draw the line where the shelves come from with a pencil.", assemblyStepsText[11]);
            Assert.Equal("Step 13: Screw the back panel of the cabinet with screw 101206.", assemblyStepsText[12]);
            Assert.Equal("Step 14: Assemble the cabinet with the help of a hammer.", assemblyStepsText[13]);
            Assert.Equal("Step 15: Fix the cabinet to the wall with piece 106989.", assemblyStepsText[14]);
            Assert.Equal("Step 16: Attach the part numbered 109336 to the doors of the cabinet with the screws numbered 105248.", assemblyStepsText[15]);
            Assert.Equal("Step 17: Mount the cabinet doors to the cabinet with screw number 105248.", assemblyStepsText[16]);
            Assert.Equal("Step 18: Mount the handle of the cabinet to the doors of the cabinet with the screw number 109567.", assemblyStepsText[17]);
        }

        /** @brief Method to generate a string of lorem ipsum text of a certain size.**/
        private string GenerateLoremIpsum(long targetSizeInBytes)
        {
            /** @brief The base string of lorem ipsum text.**/
            string loremIpsumBase = "Lorem ipsum dolor sit amet, consectetur adipiscing elit...";
            /** @brief Initialize a StringBuilder to build the final string.**/
            StringBuilder loremIpsum = new StringBuilder();

            /** @brief Loop until the length of the string is less than the target size.**/
            while (loremIpsum.Length < targetSizeInBytes)
            {
                /** @brief Append the base string to the StringBuilder.**/
                loremIpsum.Append(loremIpsumBase);
            }

            /** @brief Return the string up to the target size, ensuring it doesn't exceed the desired size.**/
            return loremIpsum.ToString().Substring(0, (int)targetSizeInBytes);
        }


        /** @brief Method to get the file size in bytes.**/
        private long FileSizeInBytes(string filePath)
        {
            /** @brief Create a FileInfo object for the specified file path.**/
            FileInfo fileInfo = new FileInfo(filePath);
            /** @brief Return the length of the file in bytes.**/
            return fileInfo.Length;
        }


  /**
   *  This class includes Xunit testing for the Bellman Ford algorithm
   */
        private readonly ITestOutputHelper _output;

        /**
        * @brief this constructor receives an ITestOutputHelper object
        * @param output ITestOutputHelper Uses  to print test outputs
        */
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }
    }
}

