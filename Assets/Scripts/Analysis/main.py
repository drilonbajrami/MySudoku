import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

# Run methods in terminal
# python -c 'from main import methodName; methodName()'

# python -c 'from main import PlotDifficultyScoreVersusGenerationTime; PlotDifficultyScoreVersusGenerationTime()'
def PlotDifficultyScoreVersusGenerationTime():
    # Read the CSV file
    data = pd.read_csv('Data/Version1.0/SudokuGeneratorV1.0.csv')

    # Filter the data based on 'Time (ms)'
    # filtered_data = data[data['Time (ms)'] < 3000]

    # Extract the columns of interest
    difficulty_score = data['Difficulty Score']
    time_ms = data['Time (ms)']

    # Set up the colormap
    cmap = 'coolwarm'

    # Highlight the difficulty score ranges with different colors
    highlight_ranges = [
        ('Beginner', (3600, 4500), 'blue'),
        ('Easy', (4300, 5500), 'green'),
        ('Medium', (5300, 6900), 'orange'),
        ('Hard', (6500, 9300), 'purple'),
    ]

    # Initialize dictionaries to store fastest and slowest times for each difficulty
    fastest_times = {}
    slowest_times = {}

    # Plot the data
    plt.scatter(difficulty_score, time_ms, c=time_ms, cmap=cmap)

    for name, (start, end), color in highlight_ranges:
        plt.axvspan(start, end, alpha=0.1, color=color, label=name)
        # Filter data for each difficulty level
        filtered_data = data[(data['Difficulty Score'] >= start) & (data['Difficulty Score'] <= end)]
        # Find the fastest and slowest times for the difficulty level
        fastest_time = filtered_data['Time (ms)'].min()
        slowest_time = filtered_data['Time (ms)'].max()
        # Store the fastest and slowest times in the dictionaries
        fastest_times[name] = fastest_time
        slowest_times[name] = slowest_time

    plt.xlabel('Difficulty Score')
    plt.ylabel('Time (ms)')
    plt.title('Difficulty Score vs. Time (ms) - 1000 Puzzles generated per level')

    plt.legend()

    # Create a table with the fastest and slowest times for each difficulty level
    table_data = {'Difficulty': [], 'Fastest Time (ms)': [], 'Slowest Time (ms)': []}
    for name in fastest_times:
        table_data['Difficulty'].append(name)
        table_data['Fastest Time (ms)'].append(fastest_times[name])
        table_data['Slowest Time (ms)'].append(slowest_times[name])

    table_df = pd.DataFrame(table_data)

    # Display the table underneath the graph 
    fig, ax = plt.subplots(figsize=(8, 4))
    ax.axis('off')
    ax.table(cellText=table_df.values, colLabels=table_df.columns, cellLoc='center', loc='center')
    plt.subplots_adjust(bottom=0.2)

    plt.show()

# python -c 'from main import PlotGenerationTimeVersusRecursiveSolverTime; PlotGenerationTimeVersusRecursiveSolverTime()'
def PlotGenerationTimeVersusRecursiveSolverTime():
    # Read the CSV file
    data = pd.read_csv('Data/Version1.0/SudokuGeneratorV1.0-RecursiveSolver.csv')

    # Filter the data based on 'Time (ms)'
    # filtered_data = data[data['Time (ms)'] < 3000]

    # Extract the columns of interest
    difficulty_score = data['Difficulty Score']
    time_ms = data['Time (ms)']
    rec_time_ms = data['Recursive Solver Time (ms)']

    # Highlight the difficulty score ranges with different colors
    highlight_ranges = [
        ('Beginner', (3600, 4500), 'blue'),
        ('Easy', (4300, 5500), 'green'),
        ('Medium', (5300, 6900), 'orange'),
        ('Hard', (6500, 9300), 'purple'),
    ]

    # Calculate the percentage of rec_time_ms compared to time_ms
    percentage = (rec_time_ms / time_ms) * 100

    # Create a colormap ranging from red to blue
    colormap = 'coolwarm'

    # Normalize the percentage values to range from 0 to 1 for colormap
    normalized_percentage = (percentage - np.min(percentage)) / (np.max(percentage) - np.min(percentage))

    for name, (start, end), color in highlight_ranges:
        plt.axvspan(start, end, alpha=0.1, color=color, label=name)

    # Plot the scatter plot
    plt.scatter(difficulty_score, percentage, c=normalized_percentage, cmap=colormap)
    
    # Set the colormap as the colorbar
    cbar = plt.colorbar()
    cbar.set_label('Percentage')

    # Set labels and title
    plt.xlabel('Difficulty Score')
    plt.ylabel('Percentage')
    plt.title('Percentage of recursive solver time spent on total generation time')

    plt.legend()
    # Show the plot
    plt.show()

# python -c 'from main import PlotGenerationTimeVersusRecursiveSolverTimeAverage; PlotGenerationTimeVersusRecursiveSolverTimeAverage()'
def PlotGenerationTimeVersusRecursiveSolverTimeAverage():
    # Read the CSV file
    data = pd.read_csv('Data/Version1.0/SudokuGeneratorV1.0-RecursiveSolver.csv')

    # Group data by difficulty score and calculate the average percentage for each difficulty score
    grouped_data = data.groupby('Difficulty Score').apply(lambda x: np.mean(x['Recursive Solver Time (ms)'] / x['Time (ms)']) * 100)

    # Create a colormap ranging from red to blue
    colormap = 'coolwarm'

    # Normalize the average percentage values to range from 0 to 1 for colormap
    normalized_percentage = (grouped_data - np.min(grouped_data)) / (np.max(grouped_data) - np.min(grouped_data))

    # Highlight the difficulty score ranges with different colors
    highlight_ranges = [
        ('Beginner', (3600, 4500), 'blue'),
        ('Easy', (4300, 5500), 'green'),
        ('Medium', (5300, 6900), 'orange'),
        ('Hard', (6500, 9300), 'purple'),
    ]

    for name, (start, end), color in highlight_ranges:
        plt.axvspan(start, end, alpha=0.1, color=color, label=name)

    # Plot the scatter plot with only the average points colored with the colormap
    plt.scatter(grouped_data.index, grouped_data.values, c=normalized_percentage, cmap=colormap, marker='o', s=100, label='Average')

    # Set the colormap as the colorbar
    cbar = plt.colorbar()
    cbar.set_label('Percentage')

    # Set labels and title
    plt.xlabel('Difficulty Score')
    plt.ylabel('Percentage')
    plt.title('Average percentage of recursive solver time spent on total generation time')

    plt.legend()
    # Show the plot
    plt.show()
