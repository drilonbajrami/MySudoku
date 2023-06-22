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
colorMap = 'winter'    

# Define the colors in the desired sequence
colors = ['#2daf2d', '#2850af', '#af3232']

# Create a custom colormap using LinearSegmentedColormap
colorMap = mcolors.LinearSegmentedColormap.from_list('custom_colormap', colors)

def PlotRanges():
    for name, (start, end), color in highlight_ranges:
        plt.axvspan(start, end, alpha=0.1, color=color, label=name)





# Run methods in terminal
# python -c 'from main import methodName; methodName()'

# python -c 'from main import PlotDifficultyScoreVersusGenerationTimeIndividual; PlotDifficultyScoreVersusGenerationTimeIndividual()'
def PlotDifficultyScoreVersusGenerationTimeIndividual():
    PlotRanges()

    # Read the CSV file
    data = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0.csv')

    filtered_data = data[(data['Time (ms)'] < 1000)]

    # Extract the columns of interest
    difficulty_score = filtered_data['Difficulty Score']
    time_ms = filtered_data['Time (ms)']

    # Plot the data
    plt.scatter(difficulty_score, time_ms, c=time_ms, cmap=colorMap, s=20)
    plt.xlabel('Difficulty Score')
    plt.ylabel('Time (ms)')
    #plt.title(r'$\bf{Sudoku\ Generator\ Version\ 2.0\ - Individual\ Plotting\ - Unfiltered\ Data}$ - Difficulty Score vs. Time (ms) - ~1000 Puzzles Generations Per Difficulty')
    plt.title(r'$\bf{Sudoku\ Generator\ Version\ 2.0\ - Individual\ Plotting\ - Filtered\ Data\ - Generations\ <\ 1\ second}$ - Difficulty Score vs. Time (ms) - ~1000 Puzzles Generations Per Difficulty')
    plt.grid(True)
    plt.legend()
    plt.show()





# python -c 'from main import PlotDifficultyScoreVersusGenerationTimeAverage; PlotDifficultyScoreVersusGenerationTimeAverage()'
def PlotDifficultyScoreVersusGenerationTimeAverage():
    PlotRanges()
    
    # Read the CSV file
    data = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0.csv')

    dataV1 = pd.read_csv('Data/Version1.0/SudokuGeneratorV1.0.csv')
    dataV2 = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0.csv')

    # Filter the data based on difficulty score and generation time
    filtered_data = data[(data['Time (ms)'] < 1000)]

    filtered_dataV1 = dataV1[(dataV1['Time (ms)'] < 1000)]
    filtered_dataV2 = dataV2[(dataV2['Time (ms)'] < 1000)]

    # Calculate the average generation time for each difficulty score
    average_times = filtered_data.groupby('Difficulty Score')['Time (ms)'].mean()

    average_timesV1 = filtered_dataV1.groupby('Difficulty Score')['Time (ms)'].mean()
    average_timesV2 = filtered_dataV2.groupby('Difficulty Score')['Time (ms)'].mean()

    # Plot the data
    #plt.scatter(average_times.index, average_times, c=average_times, cmap=colorMap, s=20)
    plt.scatter(average_timesV1.index, average_timesV1, c='red', s=20)
    plt.scatter(average_timesV2.index, average_timesV2, c='green', s=20)

    plt.xlabel('Difficulty Score')
    plt.ylabel('Average Generation Time (ms)')
    plt.title('Average Generation Time by Difficulty Score')
    #plt.title(r'$\bf{Sudoku\ Generator\ Version\ 2.0\ - Average\ Per\ Difficulty\ Score\ Plotting\ - Unfiltered\ Data}$ - Difficulty Score vs. Time (ms) - ~1000 Puzzles Generations Per Difficulty')
    #plt.title(r'$\bf{Sudoku\ Generator\ Version\ 2.0\ - Average\ Per\ Difficulty\ Score\ Plotting\ - Filtered\ Data\ - Generations\ <\ 1\ second}$ - Difficulty Score vs. Time (ms) - ~1000 Puzzles Generations Per Difficulty')
    plt.title(r'$\bf{V1.0(Red)\ &\ V2.0(Green)\ Average\ Per\ Difficulty\ Score\ Plotting\ Difference\ - Filtered\ Data\ - Generations\ <\ 1 second}$ - Difficulty Score vs. Time (ms) - ~1000 Puzzles Generations Per Difficulty')
    plt.grid(True)
    plt.legend()
    plt.show()





# python -c 'from main import PlotGenerationTimeVersusRecursiveSolverTime; PlotGenerationTimeVersusRecursiveSolverTime()'
def PlotGenerationTimeVersusRecursiveSolverTime():
    PlotRanges()

    # Read the CSV file
    data = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0-RecursiveSolver.csv')
    
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
    plt.scatter(difficulty_score, percentage, c=normalized_percentage, cmap=colorMap, s=20)
    plt.xlabel('Difficulty Score')
    plt.ylabel('Percentage')
    plt.title(r'$\bf{Sudoku\ Generator\ Version\ 2.0\ - Individual\ Plotting}$ - Percentage of Total Generation Time Spent on Recursive Solver - ~1000 Puzzles Generations Per Difficulty')
    plt.grid(True)
    plt.legend()
    plt.show()






# python -c 'from main import PlotGenerationTimeVersusRecursiveSolverTimeAverage; PlotGenerationTimeVersusRecursiveSolverTimeAverage()'
def PlotGenerationTimeVersusRecursiveSolverTimeAverage():
    PlotRanges()

    # Read the CSV file
    dataV1 = pd.read_csv('Data/Version1.0/SudokuGeneratorV1.0-RecursiveSolver.csv')
    dataV2 = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0-RecursiveSolver.csv')
    #data = pd.read_csv('Data/Version2.0/SudokuGeneratorV2.0-RecursiveSolver.csv')

    # Group data by difficulty score and calculate the average percentage for each difficulty score
    #grouped_data = data.groupby('Difficulty Score').apply(lambda x: np.mean(x['Recursive Solver Time (ms)'] / x['Time (ms)']) * 100)

    grouped_dataV1 = dataV1.groupby('Difficulty Score').apply(lambda x: np.mean(x['Recursive Solver Time (ms)'] / x['Time (ms)']) * 100)
    grouped_dataV2 = dataV2.groupby('Difficulty Score').apply(lambda x: np.mean(x['Recursive Solver Time (ms)'] / x['Time (ms)']) * 100)
    
    # Normalize the average percentage values to range from 0 to 1 for colormap
    #normalized_percentage = (grouped_dataV1 - np.min(grouped_dataV1)) / (np.max(grouped_dataV1) - np.min(grouped_dataV1))

    # Plot the scatter plot with only the average points colored with the colormap
    plt.scatter(grouped_dataV1.index, grouped_dataV1.values, c='red', cmap=colorMap, s=20)
    plt.scatter(grouped_dataV2.index, grouped_dataV2.values, c='green', cmap=colorMap, s=20)
    plt.xlabel('Difficulty Score')
    plt.ylabel('Percentage')
    plt.title(r'$\bf{Sudoku\ Generator\ - Difference\ -\ V1.0(Red)\ &\ V2.0(Green)\ - Average\ Plotting}$ - Percentage of Total Generation Time Spent on Recursive Solver - ~1000 Puzzles Generations Per Difficulty')
    plt.grid(True)
    plt.legend()
    plt.show()