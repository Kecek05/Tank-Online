
public interface IDamageable
{
    void TakeDamage(float damage);

    void Die();
}

public interface IHealable
{
    void Heal(float amount);
}
