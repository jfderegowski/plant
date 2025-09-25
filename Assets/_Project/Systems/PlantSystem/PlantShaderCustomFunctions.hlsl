// Function to get AtlasIndex and TileIndex from Pattern and IterationNumber
void GetAtlasTileIndices_float(float Pattern, float IterationNumber, out float AtlasIndex, out float TileIndex)
{
    // Ensure we're working with integers
    int pattern = (int)Pattern;
    int iteration = (int)IterationNumber;
    
    // Total tiles available (4 atlases × 16 tiles each)
    int totalTiles = 64;
    
    // Calculate which tile to use based on pattern and iteration
    // This creates a pseudo-random but deterministic selection
    int tileSelection = ((pattern * 1009 + iteration * 2017) % totalTiles + totalTiles) % totalTiles;
    
    // Extract atlas and tile indices
    AtlasIndex = floor(tileSelection / 16.0); // Which atlas (0-3)
    TileIndex = tileSelection % 16;           // Which tile in atlas (0-15)
}

// Alternative function with better distribution
void GetAtlasTileIndicesAdvanced_float(float Pattern, float IterationNumber, out float AtlasIndex, out float TileIndex)
{
    int pattern = (int)Pattern;
    int iteration = (int)IterationNumber;
    
    // Use different prime numbers for better distribution
    int hash = pattern * 374761393 + iteration * 668265263;
    hash = hash ^ (hash >> 15);
    hash = hash * 2246822507;
    hash = hash ^ (hash >> 13);
    
    // Ensure positive result
    hash = abs(hash);
    
    // Map to our tile range
    int tileSelection = hash % 64;
    
    AtlasIndex = floor(tileSelection / 16.0);
    TileIndex = tileSelection % 16;
}

// Sequential function for more predictable patterns
void GetAtlasTileIndicesSequential_float(float Pattern, float IterationNumber, out float AtlasIndex, out float TileIndex)
{
    int pattern = (int)Pattern;
    int iteration = (int)IterationNumber;
    
    // Create a unique index for each pattern-iteration combination
    int combinedIndex = (pattern * 4 + iteration) % 64;
    
    AtlasIndex = floor(combinedIndex / 16.0);
    TileIndex = combinedIndex % 16;
}