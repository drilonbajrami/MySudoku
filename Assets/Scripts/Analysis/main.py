import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.colors as mcolors
import numpy as np

# Highlight the difficulty score ranges with different colors
highlight_ranges = [
        ('Beginner', (3600, 4500), 'blue'),
        ('Easy', (4300, 5500), 'green'),
        ('Medium', (5300, 6900), 'orange'),
        ('Hard', (6500, 9300), 'purple'),
    ]

# Set up the colormap
colorMap = 'viridis'    

# Define the colors in the desired sequence
colors = ['blue', 'green', 'orange', 'red']

# Create a custom colormap using LinearSegmentedColormap
colorMap = mcolors.LinearSegmentedColormap.from_list('custom_colormap', colors)

def PlotRanges():
    for name, (start, end), color in highlight_ranges:
        plt.axvspan(start, end, alpha=0.05, color=color, label=name)





# Run methods in terminal
# python -c 'from main import methodName; methodName()'

# python -c 'from main import PlotDifficultyScoreVersusGenerationTimeIndividual; PlotDifficultyScoreVersusGenerationTimeIndividual()'
def PlotDifficultyScoreVersusGenerationTimeIndividual():
    PlotRanges()

    # Read the CSV file
    data = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0.csv')

     # Filter the data based on difficulty scores below 9300
    filtered_data = data[(data['Difficulty Score'] < 10000)]

    #& (data['Time (ms)'] < 1000)]

    # Extract the columns of interest
    difficulty_score = filtered_data['Difficulty Score']
    time_ms = filtered_data['Time (ms)']

    # Plot the data
    plt.scatter(difficulty_score, time_ms, c=time_ms, cmap=colorMap)
    plt.xlabel('Difficulty Score')
    plt.ylabel('Time (ms)')
    plt.title('Difficulty Score vs. Time (ms) - 1000 Puzzles generated per level')
    plt.legend()
    plt.show()





# python -c 'from main import PlotDifficultyScoreVersusGenerationTimeAverage; PlotDifficultyScoreVersusGenerationTimeAverage()'
def PlotDifficultyScoreVersusGenerationTimeAverage():
    PlotRanges()
    
    # Read the CSV file
    data = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0.csv')

    # Filter the data based on difficulty score and generation time
    filtered_data = data[(data['Difficulty Score'] < 10000) & (data['Time (ms)'] < 3.00)]

    # Calculate the average generation time for each difficulty score
    average_times = filtered_data.groupby('Difficulty Score')['Time (ms)'].mean()

    # Plot the data
    #plt.scatter(average_times.index, average_times, marker='o', s=50)
    plt.scatter(average_times.index, average_times, c=average_times, cmap=colorMap)
    plt.xlabel('Difficulty Score')
    plt.ylabel('Average Generation Time (ms)')
    plt.title('Average Generation Time by Difficulty Score')
    plt.legend()
    plt.show()





# python -c 'from main import PlotGenerationTimeVersusRecursiveSolverTime; PlotGenerationTimeVersusRecursiveSolverTime()'
def PlotGenerationTimeVersusRecursiveSolverTime():
    PlotRanges()

    # Read the CSV file
    data = pd.read_csv('Data/Version1.0/SudokuGeneratorV1.0-RecursiveSolverAfter.csv')
    
    # Filter the data based on 'Time (ms)'
    # filtered_data = data[data['Time (ms)'] < 3000]

    # Extract the columns of interest
    difficulty_score = data['Difficulty Score']
    time_ms = data['Time (ms)']
    rec_time_ms = data['Recursive Solver Time (ms)']

    # Calculate the percentage of rec_time_ms compared to time_ms
    percentage = (rec_time_ms / time_ms) * 100

    # Normalize the percentage values to range from 0 to 1 for colormap
    normalized_percentage = (percentage - np.min(percentage)) / (np.max(percentage) - np.min(percentage))

    # Plot the scatter plot
    plt.scatter(difficulty_score, percentage, c=normalized_percentage, cmap=colorMap)
    plt.xlabel('Difficulty Score')
    plt.ylabel('Percentage')
    plt.title('Percentage of recursive solver time spent on total generation time')
    plt.legend()
    plt.show()






# python -c 'from main import PlotGenerationTimeVersusRecursiveSolverTimeAverage; PlotGenerationTimeVersusRecursiveSolverTimeAverage()'
def PlotGenerationTimeVersusRecursiveSolverTimeAverage():
    PlotRanges()

    # Read the CSV file
    data = pd.read_csv('Data/Version1.0/SudokuGeneratorV1.0-RecursiveSolverAfter.csv')

    # Group data by difficulty score and calculate the average percentage for each difficulty score
    grouped_data = data.groupby('Difficulty Score').apply(lambda x: np.mean(x['Recursive Solver Time (ms)'] / x['Time (ms)']) * 100)

    # Normalize the average percentage values to range from 0 to 1 for colormap
    normalized_percentage = (grouped_data - np.min(grouped_data)) / (np.max(grouped_data) - np.min(grouped_data))

    # Plot the scatter plot with only the average points colored with the colormap
    plt.scatter(grouped_data.index, grouped_data.values, c=normalized_percentage, cmap=colorMap, marker='o', s=100, label='Average')
    plt.xlabel('Difficulty Score')
    plt.ylabel('Percentage')
    plt.title('Average percentage of recursive solver time spent on total generation time')
    plt.legend()
    plt.show()