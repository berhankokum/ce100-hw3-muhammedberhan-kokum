using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ce100_hw3_algo_lib_cs {
/**
* @file Class1.cs
* @author Muhammed Berhan Kokum - Muhammed Enes Celik
* @date 13 May 2023
*
* @brief <b> HW-3 Functions </b>
*
* HW-3 Sample Lib Functions
*
* @see http://bilgisayar.mmf.erdogan.edu.tr/en/
*
*/

/**
*   @name   Huffman Coding
*
*   @brief Huffman Coding  Function
*
*   The Huffman Code is an entropy coding algorithm used in computer science for data compression.
*   **/
public class HuffmanNode : IComparable<HuffmanNode> {
  /** @brief Defining a character property called "*Character". This represents the character at the node of the Huffman tree.**/
  public char Character {
    get;
    set;
  }

  /** @brief Defining a byte property called Character2. This can represent the second character in the node of the Huffman tree. **/
  public byte Character2 {
    get;
    set;
  }
  /** @brief Defining an integer property called "Frequency". This represents the frequency (i.e. the number of repetitions in the text) of a given character. **/
  public int Frequency {
    get;
    set;
  }
  /** @brief Defining a HuffmanNode property called "Left". This represents the left child of this node of the Huffman tree **/
  public HuffmanNode Left {
    get;
    set;
  }
  /** @brief Defining a HuffmanNode property called "Right". This represents the right child node of this node of the Huffman tree. **/
  public HuffmanNode Right {
    get;
    set;
  }

  /** @brief Defining a method called "CompareTo". This method comes from the "IComparable" interface and is used to compare objects of this class. **/
  /** @brief In this case, objects are compared according to the "Frequency" property. **/
  public int CompareTo(HuffmanNode other) {
    return Frequency.CompareTo(other.Frequency);
  }
}

public class HuffmanCoding {
  /** @brief Define two dictionaries to hold the Huffman codes for characters and mp3 bytes respectively**/
  private Dictionary<char, string> _huffmanCodes = new Dictionary<char, string>();
  private Dictionary<byte, string> _huffmanCodesmp3 = new Dictionary<byte, string>();

  /**
  *   @name   Compress
  *
  *   @brief Huffman Coding compress Function
  *
  *   In this method .txt files are compressed.
  *
  *   @param  [in] mass [inputFilePath, outputFilePath string]  Compress Function
  *

  **/
  public void Compress(string inputFilePath, string outputFilePath) {
    /** @brief Read the content of the input file**/
    string content = File.ReadAllText(inputFilePath);
    /** @brief Calculate the frequency of each character in the content**/
    var frequencies = CalculateFrequencies(content);
    /** @brief Build the Huffman tree based on the calculated frequencies**/
    var huffmanTreeRoot = BuildHuffmanTree(frequencies);
    /** @brief Generate the Huffman codes from the Huffman tree**/
    GenerateHuffmanCodes(huffmanTreeRoot, string.Empty);
    /** @brief Initialize a StringBuilder to hold the compressed data**/
    StringBuilder compressedData = new StringBuilder();

    /** @brief Append the Huffman code for each character in the content to the compressed data**/
    foreach (char character in content) {
      compressedData.Append(_huffmanCodes[character]);
    }

    /** @brief Calculate the size of the padding needed to make the compressed data a multiple of 8**/
    int paddingSize = 8 - (compressedData.Length % 8);

    /** @brief If padding is needed, append '0's to the compressed data**/
    if (paddingSize < 8) {
      compressedData.Append('0', paddingSize);
    }

    /** @brief Open a FileStream to write the compressed data to the output file**/
    using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create))
    /** @brief Use a BinaryWriter to write to the FileStream**/
    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream)) {
      /** @brief Write the Huffman tree to the output file**/
      WriteHuffmanTree(binaryWriter, huffmanTreeRoot);
      /** @brief Convert the compressed data to bytes**/
      byte[] compressedDataBytes = ConvertToBytes(compressedData.ToString());
      /** @brief Write the original length of the compressed data (without the padding) to the output file**/
      binaryWriter.Write(compressedData.Length - paddingSize);
      /** @brief Write the length of the compressed data bytes to the output file**/
      binaryWriter.Write(compressedDataBytes.Length);
      /** @brief Write the compressed data bytes to the output file**/
      binaryWriter.Write(compressedDataBytes);
      /** @brief Write the padding size to the output file**/
      binaryWriter.Write(paddingSize);
    }
  }
  /**
  *   @name   Compress Music
  *
  *   @brief Huffman Coding compress Function
  *
  *   In this method .MP3 files are compressed.
  *
  *   @param  [in] mass [inputFilePath, outputFilePath string]  Compress Function
  *
  **/
  public void CompressMusic(string inputFilePath, string outputFilePath) {
    byte[] content = File.ReadAllBytes(inputFilePath);
    var frequencies = CalculateFrequenciesMusic(content);
    var huffmanTreeRoot = BuildHuffmanTreeMusic(frequencies);
    GenerateHuffmanCodesMusic(huffmanTreeRoot, string.Empty);
    StringBuilder compressedData = new StringBuilder();

    foreach (byte character in content) {
      compressedData.Append(_huffmanCodesmp3[character]);
    }

    using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create))
    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream)) {
      WriteHuffmanTreeMusic(binaryWriter, huffmanTreeRoot);
      binaryWriter.Write(compressedData.Length);
      byte[] compressedDataBytes = ConvertToBytes(compressedData.ToString());
      binaryWriter.Write(compressedDataBytes.Length);
      binaryWriter.Write(compressedDataBytes);
    }
  }
  /**
  *   @name   Decompress
  *
  *   @brief Huffman Coding Decompress Function
  *
  *   In this method .txt files are decompressed.
  *
  *   @param  [in] mass [inputFilePath, outputFilePath string]  Decompress Function
  *
  **/
  public void Decompress(string inputFilePath, string outputFilePath) {
    /** @brief  Open a FileStream to read the compressed data from the input file**/
    using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open))
    /** @brief  Use a BinaryReader to read from the FileStream**/
    using (BinaryReader binaryReader = new BinaryReader(fileStream)) {
      /** @brief  Read the Huffman tree from the input file**/
      HuffmanNode huffmanTreeRoot = ReadHuffmanTree(binaryReader);
      /** @brief  Read the original length of the compressed data (without the padding) from the input file**/
      int compressedDataLength = binaryReader.ReadInt32();
      /** @brief  Read the length of the compressed data bytes from the input file**/
      int compressedDataBytesLength = binaryReader.ReadInt32();
      /** @brief  Read the compressed data bytes from the input file**/
      byte[] compressedDataBytes = binaryReader.ReadBytes(compressedDataBytesLength);
      /** @brief  Convert the compressed data bytes to a string and remove any padding**/
      string compressedData = ConvertToString(compressedDataBytes).Substring(0, compressedDataLength);
      /** @brief  Initialize a StringBuilder to hold the decompressed data**/
      StringBuilder decompressedData = new StringBuilder();
      /** @brief  Define a variable to hold the current node in the Huffman tree**/
      HuffmanNode currentNode = huffmanTreeRoot;

      /** @brief  Iterate over each bit in the compressed data**/
      foreach (char bit in compressedData) {
        /** @brief  If the bit is '0', move to the left child of the current node, otherwise move to the right child**/
        currentNode = bit == '0' ? currentNode.Left : currentNode.Right;

        /** @brief  If the current node is a leaf node, append its character to the decompressed data and reset the current node to the root of the Huffman tree**/
        if (currentNode.Left == null && currentNode.Right == null) {
          decompressedData.Append(currentNode.Character);
          currentNode = huffmanTreeRoot;
        }
      }

      /** @brief  Write the decompressed data to the output file**/
      File.WriteAllText(outputFilePath, decompressedData.ToString());
    }
  }
  /**
  *   @name   Decompress Music
  *
  *   @brief Huffman Coding Decompress Function
  *
  *   In this method .mp3 files are decompressed.
  *
  *   @param  [in] mass [inputFilePath, outputFilePath string]  Decompress Function
  *
  **/
  public void DecompressMusic(string inputFilePath, string outputFilePath) {
    /** @brief  Open a FileStream to read the compressed data from the input file**/
    using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open))
    /** @brief  Use a BinaryReader to read from the FileStream**/
    using (BinaryReader binaryReader = new BinaryReader(fileStream)) {
      /** @brief  Read the Huffman tree from the input file**/
      HuffmanNode huffmanTreeRoot = ReadHuffmanTreeMusic(binaryReader);
      /** @brief  Read the original length of the compressed data (without the padding) from the input file**/
      int compressedDataLength = binaryReader.ReadInt32();
      /** @brief  Read the length of the compressed data bytes from the input file**/
      int compressedDataBytesLength = binaryReader.ReadInt32();
      /** @brief  Read the compressed data bytes from the input file**/
      byte[] compressedDataBytes = binaryReader.ReadBytes(compressedDataBytesLength);
      /** @brief  Convert the compressed data bytes to a string and remove any padding**/
      string compressedData = ConvertToString(compressedDataBytes).Substring(0, compressedDataLength);
      /** @brief  Initialize a list to hold the decompressed data**/
      List<byte> decompressedData = new List<byte>();
      /** @brief  Define a variable to hold the current node in the Huffman tree**/
      HuffmanNode currentNode = huffmanTreeRoot;

      /** @brief  Iterate over each bit in the compressed data**/
      foreach (char bit in compressedData) {
        /** @brief  If the bit is '0', move to the left child of the current node, otherwise move to the right child**/
        currentNode = bit == '0' ? currentNode.Left : currentNode.Right;

        /** @brief  If the current node is a leaf node, append its byte to the decompressed data and reset the current node to the root of the Huffman tree**/
        if (currentNode.Left == null && currentNode.Right == null) {
          decompressedData.Add(currentNode.Character2);
          currentNode = huffmanTreeRoot;
        }
      }

      /** @brief  Write the decompressed data to the output file**/
      File.WriteAllBytes(outputFilePath, decompressedData.ToArray());
    }
  }
  /**
  *   @name   Calculate Frequencies
  *
  *   @brief Huffman Coding CalculateFrequencies Function
  *
  *   Method to calculate the frequencies of each character in a string
  *
  *   @param  [in] mass [content string]  Calculate Frequencies Function
  *
  **/
  private Dictionary<char, int> CalculateFrequencies(string content) {
    /** @brief  Initialize a dictionary to hold the frequencies**/
    Dictionary<char, int> charFrequencies = new Dictionary<char, int>();

    /** @brief  Iterate over each character in the string**/
    foreach (char character in content) {
      /** @brief  If the character is already in the dictionary, increment its frequency**/
      if (charFrequencies.ContainsKey(character)) {
        charFrequencies[character]++;
      }
      /** @brief  If the character is not in the dictionary, add it with a frequency of 1**/
      else {
        charFrequencies[character] = 1;
      }
    }

    /** @brief  Return the dictionary of frequencies**/
    return charFrequencies;
  }

  /**
  *   @name   Calculate Frequencies Music
  *
  *   @brief Huffman Coding CalculateFrequenciesMusic Function
  *
  *   Method to calculate the frequencies of each byte in a byte array
  *
  *   @param  [in] mass [content string]  Calculate Frequencies Function Music
  *
  **/
  private Dictionary<byte, int> CalculateFrequenciesMusic(byte[] content) {
    /** @brief  Initialize a dictionary to hold the frequencies**/
    Dictionary<byte, int> byteFrequencies = new Dictionary<byte, int>();

    /** @brief  Iterate over each byte in the byte array**/
    foreach (byte b in content) {
      /** @brief  If the byte is already in the dictionary, increment its frequency**/
      if (byteFrequencies.ContainsKey(b)) {
        byteFrequencies[b]++;
      }
      /** @brief  If the byte is not in the dictionary, add it with a frequency of 1**/
      else {
        byteFrequencies[b] = 1;
      }
    }

    /** @brief  Return the dictionary of frequencies**/
    return byteFrequencies;
  }

  /**
  *   @name   Build Huffman Tree
  *
  *   @brief Huffman Coding BuildHuffmanTree Function
  *
  *   Method to build the Huffman tree from a dictionary of character frequencies
  *
  *   @param  [in] mass [frequencies Dictionary<char, int>]  Build Huffman Tree
  *
  **/
  private HuffmanNode BuildHuffmanTree(Dictionary<char, int> frequencies) {
    /** @brief  Create a priority queue of Huffman nodes, each node is associated with a character and its frequency**/
    var priorityQueue = new List<HuffmanNode>(frequencies.Select(f => new HuffmanNode {
      Character = f.Key,
      Frequency = f.Value
    }));

    /** @brief  Continue until there is only one node left in the priority queue (the root of the Huffman tree)**/
    while (priorityQueue.Count > 1) {
      /** @brief  Order the priority queue by frequency**/
      priorityQueue = priorityQueue.OrderBy(HuffmanNode => HuffmanNode.Frequency).ToList();
      /** @brief  The two nodes with the lowest frequency are combined into a new node**/
      HuffmanNode left = priorityQueue[0];
      HuffmanNode right = priorityQueue[1];
      /** @brief  Create a new node with the combined frequency of the two lowest nodes, and these two nodes as its children**/
      HuffmanNode newNode = new HuffmanNode {
        Frequency = left.Frequency + right.Frequency,
        Left = left,
        Right = right
      };
      /** @brief  Remove the two lowest nodes from the priority queue**/
      priorityQueue.Remove(left);
      priorityQueue.Remove(right);
      /** @brief  Add the new node to the priority queue**/
      priorityQueue.Add(newNode);
    }

    /** @brief Return the remaining node in the priority queue, which is the root of the Huffman tree**/
    return priorityQueue.Single();
  }

  /**
  *   @name   Build Huffman Tree Music
  *
  *   @brief Huffman Coding BuildHuffmanTreeMusic Function
  *
  *   Method to build the Huffman tree from a dictionary of byte frequencies
  *
  *   @param  [in] mass [frequencies Dictionary<char, int>]  Build Huffman Tree Music
  *
  **/
  private HuffmanNode BuildHuffmanTreeMusic(Dictionary<byte, int> frequencies) {
    /** @brief  Create a priority queue of Huffman nodes, each node is associated with a byte and its frequency**/
    var priorityQueue = new List<HuffmanNode>(frequencies.Select(f => new HuffmanNode {
      Character2 = f.Key,
      Frequency = f.Value
    }));

    /** @brief  Continue until there is only one node left in the priority queue (the root of the Huffman tree)**/
    while (priorityQueue.Count > 1) {
      /** @brief  Order the priority queue by frequency**/
      priorityQueue = priorityQueue.OrderBy(node => node.Frequency).ToList();
      /** @brief  The two nodes with the lowest frequency are combined into a new node**/
      HuffmanNode left = priorityQueue[0];
      HuffmanNode right = priorityQueue[1];
      /** @brief  Create a new node with the combined frequency of the two lowest nodes, and these two nodes as its children**/
      HuffmanNode newNode = new HuffmanNode {
        Frequency = left.Frequency + right.Frequency,
        Left = left,
        Right = right
      };
      /** @brief  Remove the two lowest nodes from the priority queue**/
      priorityQueue.Remove(left);
      priorityQueue.Remove(right);
      /** @brief  Add the new node to the priority queue**/
      priorityQueue.Add(newNode);
    }

    /** @brief  Return the remaining node in the priority queue, which is the root of the Huffman tree**/
    return priorityQueue.Single();
  }

  /**
  *   @name   Generate Huffman Codes
  *
  *   @brief Huffman Coding GenerateHuffmanCodes Function
  *
  *   Method to generate Huffman codes for each character using the Huffman tree
  *
  *   @param  [in] mass [code string]  Generate Huffman Codes
  *
  **/
  private void GenerateHuffmanCodes(HuffmanNode node, string code) {
    /** @brief  If the node is null, return immediately**/
    if (node == null) {
      return;
    }

    /** @brief  If the node is a leaf node (i.e., it has no children), **/
    /** @brief  then it represents a character, and we assign the current code to this character**/
    if (node.Left == null && node.Right == null) {
      _huffmanCodes[node.Character] = code;
    }

    /** @brief  Recursively generate codes for the left and right children, **/
    /** @brief  appending '0' for the left child and '1' for the right child**/
    GenerateHuffmanCodes(node.Left, code + "0");
    GenerateHuffmanCodes(node.Right, code + "1");
  }

  /**
  *   @name   Generate Huffman Codes Music
  *
  *   @brief Huffman Coding GenerateHuffmanCodesMusic Function
  *
  *   Method to generate Huffman codes for each byte using the Huffman tree.
  *
  *   @param  [in] mass [code string]  Generate Huffman Codes Music
  *
  **/
  private void GenerateHuffmanCodesMusic(HuffmanNode node, string code) {
    /** @brief  If the node is null, return immediately**/
    if (node == null) {
      return;
    }

    /** @brief  If the node is a leaf node (i.e., it has no children), **/
    /** @brief  then it represents a byte, and we assign the current code to this byte**/
    if (node.Left == null && node.Right == null) {
      _huffmanCodesmp3[node.Character2] = code;
    }

    /** @brief  Recursively generate codes for the left and right children, **/
    /** @brief  appending '0' for the left child and '1' for the right child**/
    GenerateHuffmanCodesMusic(node.Left, code + "0");
    GenerateHuffmanCodesMusic(node.Right, code + "1");
  }

  /**
  *   @name   Read Huffman Tree
  *
  *   @brief Huffman Coding ReadHuffmanTree Function
  *
  *   Method to reconstruct the Huffman tree from binary data.
  *
  *   @param  [in] mass [binaryReader BinaryReader]  Read Huffman Tree
  *
  **/
  private HuffmanNode ReadHuffmanTree(BinaryReader binaryReader) {
    /** @brief  If there are no more bytes to read, return null**/
    if (binaryReader.PeekChar() == -1) {
      return null;
    }

    /** @brief  Read the node type (1 for leaf nodes, other values for internal nodes)**/
    byte nodeType = binaryReader.ReadByte();

    /** @brief  If the node type is 1, it's a leaf node, so we read the character**/
    if (nodeType == 1) {
      /** @brief  Read the character from the binary data**/
      char character = (char)binaryReader.ReadUInt16();
      /** @brief  Create and return a new leaf node with this character**/
      return new HuffmanNode { Character = character };
    } else {
      /** @brief  If the node type is not 1, it's an internal node, so we read the left and right children**/
      /** @brief  Recursively read the left child**/
      HuffmanNode left = ReadHuffmanTree(binaryReader);
      /** @brief  Recursively read the right child**/
      HuffmanNode right = ReadHuffmanTree(binaryReader);
      /** @brief  Create and return a new internal node with these children**/
      return new HuffmanNode { Left = left, Right = right };
    }
  }

  /**
  *   @name   Read Huffman Tree Music
  *
  *   @brief Huffman Coding ReadHuffmanTreeMusic Function
  *
  *   Method to reconstruct the Huffman tree from binary data for music decompression.
  *
  *   @param  [in] mass [binaryReader BinaryReader]  Read Huffman Tree Music
  *
  **/
  private HuffmanNode ReadHuffmanTreeMusic(BinaryReader binaryReader) {
    /** @brief  If there are no more bytes to read, return null**/
    if (binaryReader.PeekChar() == -1) {
      return null;
    }

    /** @brief  Read the node type (1 for leaf nodes, other values for internal nodes)**/
    byte nodeType = binaryReader.ReadByte();

    /** @brief  If the node type is 1, it's a leaf node, so we read the byte**/
    if (nodeType == 1) {
      /** @brief  Read the byte from the binary data**/
      byte character = binaryReader.ReadByte();
      /** @brief  Create and return a new leaf node with this byte**/
      return new HuffmanNode { Character2 = character };
    } else {
      /** @brief  If the node type is not 1, it's an internal node, so we read the left and right children**/
      /** @brief  Recursively read the left child**/
      HuffmanNode left = ReadHuffmanTreeMusic(binaryReader);
      /** @brief  Recursively read the right child**/
      HuffmanNode right = ReadHuffmanTreeMusic(binaryReader);
      /** @brief  Create and return a new internal node with these children**/
      return new HuffmanNode { Left = left, Right = right };
    }
  }

  /**
  *   @name   Convert to String
  *
  *   @brief Huffman Coding ConvertToString Function
  *
  *   Method to convert a byte array into a string of binary digits
  *
  *   @param  [in] mass [bytes byte]  Convert to String
  *
  **/
  private string ConvertToString(byte[] bytes) {
    /** @brief  Initialize a StringBuilder with capacity equal to the number of bytes times 8 (since each byte has 8 bits)**/
    StringBuilder binaryString = new StringBuilder(bytes.Length * 8);

    /** @brief  For each byte in the input array**/
    foreach (byte b in bytes) {
      /** @brief  Convert the byte to a binary string (base 2), pad with leading zeros to ensure it's always 8 digits long, **/
      /** @brief  and append it to the StringBuilder**/
      binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
    }

    /** @brief  Return the binary string**/
    return binaryString.ToString();
  }

  /**
  *   @name   Write Huffman Tree
  *
  *   @brief Huffman Coding WriteHuffmanTree Function
  *
  *   Method to write the Huffman tree to binary data.
  *
  *   @param  [in] mass [binaryWriter,node BinaryWriter, HuffmanNode]  Write Huffman Tree
  *

  **/
  private void WriteHuffmanTree(BinaryWriter binaryWriter, HuffmanNode node) {
    /** @brief  If the node is null, we've reached a leaf node, so return**/
    if (node == null) {
      return;
    }

    /** @brief  If the node is a leaf node (both left and right children are null)**/
    if (node.Left == null && node.Right == null) {
      /** @brief  Write a byte value of 1 to denote a leaf node**/
      binaryWriter.Write((byte)1);
      /** @brief  Write the character of this leaf node (convert the character to ushort for writing)**/
      binaryWriter.Write((ushort)node.Character);
    } else {
      /** @brief  If the node is an internal node (it has at least one child)**/
      /** @brief  Write a byte value of 0 to denote an internal node**/
      binaryWriter.Write((byte)0);
      /** @brief  Recursively write the left child**/
      WriteHuffmanTree(binaryWriter, node.Left);
      /** @brief  Recursively write the right child**/
      WriteHuffmanTree(binaryWriter, node.Right);
    }
  }

  /**
  *   @name   Write Huffman Tree Music
  *
  *   @brief Huffman Coding WriteHuffmanTreeMusic Function
  *
  *   Method to write the Huffman tree for music to binary data.
  *
  *   @param  [in] mass [binaryWriter,node BinaryWriter, HuffmanNode]  Write Huffman Tree Music
  *
  *
  **/
  private void WriteHuffmanTreeMusic(BinaryWriter binaryWriter, HuffmanNode node) {
    /** @brief  If the node is null, we've reached a leaf node, so return**/
    if (node == null) {
      return;
    }

    /** @brief  If the node is a leaf node (both left and right children are null)**/
    if (node.Left == null && node.Right == null) {
      /** @brief  Write a byte value of 1 to denote a leaf node**/
      binaryWriter.Write((byte)1);
      /** @brief  Write the byte (Character2) of this leaf node**/
      binaryWriter.Write(node.Character2);
    } else {
      /** @brief  If the node is an internal node (it has at least one child)**/
      /** @brief  Write a byte value of 0 to denote an internal node**/
      binaryWriter.Write((byte)0);
      /** @brief  Recursively write the left child**/
      WriteHuffmanTreeMusic(binaryWriter, node.Left);
      /** @brief  Recursively write the right child**/
      WriteHuffmanTreeMusic(binaryWriter, node.Right);
    }
  }
  /**
  *   @name   Convert to Bytes
  *
  *   @brief Huffman Coding ConvertToBytes Function
  *
  *   Method to convert a binary string to an array of bytes.
  *
  *   @param  [in] mass [binaryWriter,node BinaryWriter, HuffmanNode]  Convert to Bytes
  *
  *
  **/
  private byte[] ConvertToBytes(string binaryString) {
    /** @brief  Calculate padding size - how many extra '0's need to be added to the end of the string**/
    /** @brief  to make it a multiple of 8 (length of one byte)**/
    int paddingSize = 8 - (binaryString.Length % 8);

    /** @brief  If the padding size is not 8, pad the binary string with '0's on the right**/
    if (paddingSize != 8) {
      binaryString = binaryString.PadRight(binaryString.Length + paddingSize, '0');
    }

    /** @brief  Determine the number of bytes in the binary string**/
    int numOfBytes = binaryString.Length / 8;
    /** @brief  Initialize a byte array of the determined size**/
    byte[] bytes = new byte[numOfBytes];

    /** @brief  Iterate over the binary string in chunks of 8 characters (one byte)**/
    for (int i = 0; i < binaryString.Length; i += 8) {
      /** @brief  Extract the current chunk of 8 characters**/
      string chunk = binaryString.Substring(i, Math.Min(8, binaryString.Length - i));
      /** @brief  Convert the chunk to a byte and store it in the byte array**/
      bytes[i / 8] = Convert.ToByte(chunk, 2);
    }

    /** @brief  Return the byte array**/
    return bytes;
  }
}

/**
*   @name   AssemblyStep
*
*   @brief AssemblyStep Class
*
*   Define a public class named AssemblyStep.
**/
public class AssemblyStep {
  /** @brief  Define a public integer property named StepId. This is the unique identifier for each assembly step.**/
  public int StepId {
    get;
    set;
  }

  /** @brief  Define a public string property named Description. This provides detailed information about the assembly step.**/
  public string Description {
    get;
    set;
  }

  /** @brief  Define a public list of integers property named Dependencies. Each integer in this list represents the StepId of another AssemblyStep that must be completed before this one.**/
  public List<int> Dependencies {
    get;
    set;
  }
}

/**
*   @name   IkeaAssemblyGuide
*
*   @brief IkeaAssemblyGuide Class
**/
public class IkeaAssemblyGuide {
  /** @brief  Define a private list of AssemblyStep objects. This will hold all the assembly steps in the guide.**/
  private List<AssemblyStep> assemblySteps;

  /** @brief  Constructor for the IkeaAssemblyGuide class. It initializes the list of assembly steps.**/
  public IkeaAssemblyGuide() {
    /** @brief  Initialize the list of assembly steps.**/
    assemblySteps = new List<AssemblyStep>();
  }

  /**
  *   @name   Add Assembly Step
  *
  *   @brief  AddAssemblyStep Method
  *
  *   Method to add an assembly step to the guide. It takes a step ID, a description, and a list of dependencies as arguments.
  *
  *   @param  [in] mass [stepId,description,dependencies int, string,List<int>]  Add Assembly Step
  *
  **/
  public void AddAssemblyStep(int stepId, string description, List<int> dependencies) {
    /** @brief  Create a new AssemblyStep object with the provided arguments.**/
    AssemblyStep step = new AssemblyStep {
      StepId = stepId,
      Description = description,
      Dependencies = dependencies
    };
    /** @brief  Add the new assembly step to the list of assembly steps.**/
    assemblySteps.Add(step);
  }


  /**
  *   @name   Topological Sort
  *
  *   @brief  TopologicalSort Method
  *
  *   Method to add an assembly step to the guide. It takes a step ID, a description, and a list of dependencies as arguments.
  *
  *   @param  [in] mass []  Topological Sort
  *

  **/
  public List<int> TopologicalSort() {
    /** @brief  Create a dictionary from the list of assembly steps for easy lookup by step ID.**/
    Dictionary<int, AssemblyStep> stepDict = assemblySteps.ToDictionary(step => step.StepId);
    /** @brief  Create a hash set to keep track of the steps that have been visited during the depth-first search (DFS).**/
    HashSet<int> visited = new HashSet<int>();
    /** @brief  Create a stack to hold the sorted steps. Steps are added to the stack after all their dependencies have been visited.**/
    Stack<int> sortedStack = new Stack<int>();

    /** @brief  Iterate over each step in the dictionary. If a step has not been visited, perform a DFS from that step.**/
    foreach (int stepId in stepDict.Keys) {
      if (!visited.Contains(stepId)) {
        DFS(stepId, visited, sortedStack, stepDict);
      }
    }

    /** @brief  Return the steps in the order they should be performed. Since we added steps to the stack after their dependencies, **/
    /** @brief  converting the stack to a list gives the correct order of steps.**/
    return sortedStack.ToList();
  }
  /**
  *   @name   DFS
  *
  *   @brief  DFS Method
  *
  *   This is a private helper method to perform a depth-first search (DFS) from a given step ID.
  *
  *   @param  [in] mass [stepId,visited,sortedStack,stepDict int, HashSet<int>,Stack<int>,Dictionary<int, AssemblyStep>]  DFS
  *
  **/
  private void DFS(int stepId, HashSet<int> visited, Stack<int> sortedStack, Dictionary<int, AssemblyStep> stepDict) {
    /** @brief  Add the current step ID to the set of visited steps.**/
    visited.Add(stepId);
    /** @brief  Get the current step from the dictionary using the step ID.**/
    AssemblyStep currentStep = stepDict[stepId];

    /** @brief  Iterate over each dependency of the current step.**/
    foreach (int dependencyId in currentStep.Dependencies) {
      /** @brief  If a dependency has not been visited, perform a DFS from that dependency.**/
      if (!visited.Contains(dependencyId)) {
        DFS(dependencyId, visited, sortedStack, stepDict);
      }
    }

    /** @brief  After visiting all dependencies of the current step, add the current step ID to the stack.**/
    sortedStack.Push(stepId);
  }

  /**
  *   @name   Get Assembly Steps Text
  *
  *   @brief  GetAssemblyStepsText Method
  *
  *   This method is to get the assembly steps in a proper order (based on their dependencies) in a text format.
  *
  *   @param  [in] mass []  Get Assembly Steps Text
  *
  **/
  public List<string> GetAssemblyStepsText() {
    /** @brief  Perform a topological sort to get the steps in the order they should be performed.**/
    List<int> sortedSteps = TopologicalSort();
    /** @brief  Initialize a new list to store the steps in text format.**/
    List<string> assemblyStepsText = new List<string>();

    /** @brief  Iterate over the sorted steps in reverse order. This is because the topological sort gives us the steps in reverse order (the last step comes first).**/
    for (int i = sortedSteps.Count - 1; i >= 0; i--) {
      /** @brief  Get the ID of the current step.**/
      int stepId = sortedSteps[i];
      /** @brief  Find the step with the current ID in the list of all assembly steps.**/
      AssemblyStep step = assemblySteps.First(s => s.StepId == stepId);
      /** @brief  Add the description of the current step to the list of step texts, prefixed by "Step {stepId}:".**/
      assemblyStepsText.Add($"Step {stepId}: {step.Description}");
    }

    /** @brief  Return the list of step descriptions in the correct order.**/
    return assemblyStepsText;
  }

}
/**
     * Entries that define a tree
     */
public class Tree {
  /**
   * Id of the tree entering the input
   * */
  public int Id {
    get;
    set;
  }

  /**
   * input showing the location of the tree on the X-axis
   */
  public double X {
    get;
    set;
  }

  /**
  * input showing the location of the tree on the Y-axis
  */
  public double Y {
    get;
    set;
  }
}


/**
* class representing an edge between two trees
*/
public class EdgePS {


  /**
   *Source tree located on the edge
   */
  public Tree Source {
    get;
    set;
  }

  /**
   *Target tree located on the edge
   */
  public Tree Destination {
    get;
    set;
  }

  /**
   *Input containing the distance between the target and source tree
   */
  public double Distance {
    get;
    set;
  }
}


/**
 * Represents a system that connects trees
 */
public class PipelineSystem

{
  /**
  *Keeps a list of trees in the system
  */
  private List<Tree> _trees;

  /**
  * A list of links between trees in the system
  */
  private List<EdgePS> _edges;


  /**
   * Runs a piplinesystem instance with the given number of trees
   */
  public PipelineSystem(int numberOfTrees) {
    _trees = new List<Tree>();
    _edges = new List<EdgePS>();
    /**generates the positions of trees in random shapes*/
    GenerateRandomTreeLocations(numberOfTrees);
    /**makes connections between randomly positioned trees*/
    BuildConnections();
  }

  /*
   * randomly generates the locations of the specified number of trees
   *
   * @param numberOfTrees number of trees to generate random location
   * */
  private void GenerateRandomTreeLocations(int numberOfTrees) {
    /**
    *creates a new random object
    */
    Random random = new Random();

    /*
     * A for loop that sets random locations for each tree
     * */
    for (int i = 0; i < numberOfTrees; i++) {
      _trees.Add(new Tree {
        Id = i,
        X = random.NextDouble() * 100,
        Y = random.NextDouble() * 100
      });
    }
  }

  /**
   * makes connections between randomly positioned trees
   */
  private void BuildConnections() {
    /**
     * Calculate the distance between two trees
     */
    for (int i = 0; i < _trees.Count; i++) {
      for (int j = i + 1; j < _trees.Count; j++) {
        // Calculate the distance between two trees
        double distance = Math.Sqrt(Math.Pow(_trees[i].X - _trees[j].X, 2) + Math.Pow(_trees[i].Y - _trees[j].Y, 2));
        // Add an EdgePS object containing this distance to the _edges list
        _edges.Add(new EdgePS {
          Source = _trees[i],
          Destination = _trees[j],
          Distance = distance
        });
      }
    }
  }

  /**
   * Finds the minimum spanning tree (MST) of the system using Kruskal's algorithm.
   * @returns  A list of edges representing the MST.
   */
  public List<EdgePS> KruskalMST() {
    // creates a list of edges of the MST
    List<EdgePS> mstEdges = new List<EdgePS>();
    // Creates the DisjointSet object
    DisjointSet disjointSet = new DisjointSet(_trees.Count);
    /**
     * Code sequence that sorts edges by distance
     */
    _edges.Sort((e1, e2) => e1.Distance.CompareTo(e2.Distance));

    foreach (EdgePS edge in _edges) {
      // Finds the roots of source and target trees
      int root1 = disjointSet.Find(edge.Source.Id);
      int root2 = disjointSet.Find(edge.Destination.Id);

      //If the value roots are different from each other in the for loop
      if (root1 != root2) {
        // Adds edge to MST
        mstEdges.Add(edge);
        // Combines roots
        disjointSet.Union(root1, root2);
      }
    }

    // Returns the list of edges of the MST
    return mstEdges;
  }

  /**
  *  Returns textual descriptions of the edges of the minimum spanning tree (MST).
  * @param mstEdges List of edges in MST.
  * @returns A list of textual descriptions of edges in the MST.
  */
  public List<string> GetMSTEdgesTextualDescriptions(List<EdgePS> mstEdges) {
    // Creates a list of textual descriptions
    List<string> descriptions = new List<string>();
    // Creates a textual description for each edge and adds it to the list

    foreach (EdgePS edge in mstEdges) {
      // Creates a textual description of the edge and adds it to the list
      descriptions.Add($"Tree {edge.Source.Id} ({edge.Source.X}, {edge.Source.Y}) -> Tree {edge.Destination.Id} ({edge.Destination.X}, {edge.Destination.Y}): {edge.Distance}");
    }

    // Returns a list of textual descriptions
    return descriptions;
  }
}

/**
 * Cod array representing a discrete data structure
 */
public class DisjointSet {
  private int[] _parent;
  private int[] _rank;

  /**
   * Prompts to instantiate a new instance of the DisjoinSet class with the desired size
   * @param size holds the size of the disjoint set
   */
  public DisjointSet(int size) {
    /**
     * creates _parent and _rank arrays of the specified size
     */
    _parent = new int[size];
    _rank = new int[size];

    /**
     * Sets the root of each element equal to itself and sets its rank to 0
     */
    for (int i = 0; i < size; i++) {
      _parent[i] = i;
      _rank[i] = 0;
    }
  }


  /**
   * Allows to find the root of the desired element
   * @param x is the element whose root will be found
   * @returns  root of the specified element
   */
  public int Find(int x) {
    if (_parent[x] != x) {
      _parent[x] = Find(_parent[x]);
    }

    return _parent[x];
  }


  /**
   * code sequence combining the roots of two elements
   * @param x first element to be merged
   * @param y second element to be merged
   */
  public void Union(int x, int y) {
    /**
     * Finds the roots of the first and second element
     */
    int rootX = Find(x);
    int rootY = Find(y);

    /**
     * no concatenation if the two roots have the same value
     */
    if (rootX == rootY) {
      return;
    }

    /**
     * If the rank of the first element is greater than the second, equals the root of the second to the first
     */
    if (_rank[rootX] > _rank[rootY]) {
      _parent[rootY] = rootX;
    }
    /**
     * If the rank of the first element is less than the second, equals the root of the first to the second
     */
    else if (_rank[rootX] < _rank[rootY]) {
      _parent[rootX] = rootY;
    }
    /**
     * If the ranks are equal, equals the root of the second to the first and increases the rank of the first
     */
    else {
      _parent[rootY] = rootX;
      _rank[rootX]++;
    }
  }

}

/** @brief Using the Bellman-Ford algorithm to find the shortest paths from a source origin node to all other nodes in a graph
 *  @param edges is the list of edges in the graph
 *  @param verticesCount keeps the number of nodes in the graph
 *  @param source  source node used to find the shortest path
 *  @return sequence of distances from the source node to all other nodes in the graph
*/
public class Edge {
  /** @brief Source vertex of the edge
  */
  public int Source {
    get;
    set;
  }

  /** @brief Destination vertex of the edge
  */
  public int Destination {
    get;
    set;
  }

  /** @brief Weight of the edge
  */
  public int Weight {
    get;
    set;
  }

  /** @brief Constructor for Edge class
  *  @param source Source vertex of the edge
  *  @param destination Destination vertex of the edge
  *  @param weight Weight of the edge
  */
  public Edge(int source, int destination, int weight) {
    Source = source;
    Destination = destination;
    Weight = weight;
  }
}


/** @brief BellmanFord sınıfı, applies the Bellman-Ford algorithm to find the shortest path in a graph
 */
public class BellmanFord {
  private const int INF = int.MaxValue;

  public static int[] FindShortestPath(List<Edge> edges, int verticesCount, int source) {
    /**Initialize the distances array to store the shortest distances from the source node to all other nodes
     */
    int[] distances = new int[verticesCount];

    for (int i = 0; i < verticesCount; i++) {
      /**sets start distances to infinity
       */
      distances[i] = INF;
    }

    /**
     * sets the distance from the source node to itself to 0
     */
    distances[source] = 0;

    /**
     * traverse all nodes (except source nodes) and if a shorter path is found, select that distance
     */
    for (int i = 1; i < verticesCount; i++) {
      /**
      * traverse all nodes  and if a shorter path is found, select that distance
      */
      foreach (Edge edge in edges) {
        /** gets the source node of the edge
        */
        int u = edge.Source;
        /**
         * gets the target node of the edge
         */
        int v = edge.Destination;
        /**
         * takes the weight of the edge
         */
        int weight = edge.Weight;

        /**
         * If the distance from the source node to node u is not infinite and the distance from node u to node v over this edge is shorter than the current distance from the source node to node v
         */
        if (distances[u] != INF && distances[u] + weight < distances[v]) {
          /**
           * update the distance from the source node to node v
           */
          distances[v] = distances[u] + weight;
        }
      }
    }

    /**
     * Iterate through all edges in the graph and check for negative-weight cycle
     */
    foreach (Edge edge in edges) {
      /** @brief Source vertex of the edge
      */
      int u = edge.Source;
      /** @brief Destination vertex of the edge
      */
      int v = edge.Destination;
      /** @brief Weight of the edge
      */
      int weight = edge.Weight;

      /**
       * If distance from source vertex to vertex u is not infinity and distance from vertex u to vertex v through this edge is shorter than current distance from source vertex to vertex v
       */
      if (distances[u] != INF && distances[u] + weight < distances[v]) {
        /**
         * Throw exception indicating presence of negative-weight cycle
         */
        throw new InvalidOperationException("Graph contains a negative-weight cycle");
      }
    }

    /**
     * Return array of shortest distances from source vertex to all other vertices in the graph
     */
    return distances;
  }
}
}
