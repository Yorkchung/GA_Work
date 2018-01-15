package GA;
import java.util.*;

public class GeneticAlgorithm {
  public static void run(Chromosome prototype, int size, int maxGen) {
	Population pop = new Population();
    pop.initialize(prototype, size);
    for (int genIdx=0; genIdx<maxGen; genIdx++) {
      pop = pop.reproduction();
      System.out.println("=============pop==============\n"+pop.toString());
    }
  }  
}

class Population extends Vector {
  static Random random = new Random();

  public void initialize(Chromosome prototype, int popSize) {
    clear();
    for (int i=0; i<popSize; i++) {
      Chromosome newChrom = prototype.randomInstance();
      newChrom.calcFitness();
      add(newChrom);
    }
  }
  
  public Chromosome selection() {
    int shoot  = random.nextInt((size()*size()) / 2);
    int select = (int) Math.floor(Math.sqrt(shoot*2));
    return (Chromosome) get(select);
  }

  public void sort() {
    TreeMap map = new TreeMap();
    for (int i=0; i<size(); i++) {
      Chromosome chromNow = (Chromosome) get(i);
      map.put(new Double(chromNow.fitness+i*0.000000000001), chromNow);
    }
    clear();
    addAll(map.values());
  }

  public Population reproduction() {
    sort();
    Population newPop = new Population();
    for (int i=0; i<size(); i++) {
       Chromosome parent1 = selection();
       Chromosome parent2 = selection();
       Chromosome child   = parent1.crossover(parent2);
	   double prob = random.nextFloat();
       if (prob < 0.2) child.mutate();
       child.calcFitness();
       newPop.add(child);
    }
    return newPop;
  }

  public String toString() {
  	return super.toString().replace(',', '\n');
  }
}

abstract class Chromosome {
  public double fitness;
  abstract public double calcFitness();
  abstract public Chromosome crossover(Chromosome spouse);
  abstract public void mutate();
  abstract public Chromosome randomInstance();
  abstract public Chromosome copy();
}