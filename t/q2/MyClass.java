import java.util.Date;
import java.util.List;

public class MyClass
{
    // all members can be made final
    private Date m_time;
    private String m_name;
    private List<Long> m_numbers;
    private List<String> m_strings;
    
    public MyClass(Date time, String name, List<Long> numbers, List<String> strings)
    {
        m_time = time;
        m_name = name;
        m_numbers = numbers;
        m_strings = strings;
    }

    // probably we should override the hashCode() as well

    public boolean equals(Object obj) 
    {
        // probably the class implements a snapshot of some state
        // if it is true, then the time member should be checked for equality as well

        // https://www.sitepoint.com/implement-javas-equals-method-correctly/

        // method should be made final
        // method should test for null
        // method should test for self (optimization)

        if (obj instanceof MyClass) {
            return m_name.equals(((MyClass)obj).m_name);
        }
        return false;
    }
    
    public String toString() 
    {
        // inefficient approach, the execution and memory complexity is O(n*n)
        // StringBuilder should be used here

        String out = m_name;
        for (long item : m_numbers) {
            out += " " + item;
        }
        return out;
    }
    
    public void removeString(String str) 
    {
        // wrong approach to iteration while collection is being modified
        // when string is removed from list its size decreases
        // an OutOfBounds exception here if a string is actually removed
        for (int i = 0; i < m_strings.size(); i++) {
            if (m_strings.get(i).equals(str)) {
                m_strings.remove(i);
            }
        }

        // a proper solution to remove all strings
        int i = 0;
        while (i < m_strings.size()) {
            if (m_strings.get(i).equals(str)) {
                m_strings.remove(i);
                continue;
            }
            i++;
        }

    }

    public boolean containsNumber(long number) 
    {
        for (long num : m_numbers) {
            if (num == number) {
                return true;
            }
        }
        return false;
    }
    
    // inconsistent behaviour here, two consequent calls may return different results
    // isHistoric() should get a date to compare to from outside
    public boolean isHistoric() {
        return m_time.before(new Date());
    }
}